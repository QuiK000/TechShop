using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Services.Impl;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId)
    {
        return await _context.ShoppingCartItems
            .Include(sci => sci.Product)
            .ThenInclude(p => p.Category)
            .Where(sci => sci.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> AddToCartAsync(string userId, int productId, int quantity)
    {
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsAvailable || product.StockQuantity < quantity)
                return false;
            
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(sci => sci.UserId == userId && sci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                
                if (existingItem.Quantity > product.StockQuantity)
                    existingItem.Quantity = product.StockQuantity;
            }
            else
            {
                var cartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                };
                _context.ShoppingCartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
    {
        try
        {
            var cartItem = await _context.ShoppingCartItems
                .Include(sci => sci.Product)
                .FirstOrDefaultAsync(sci => sci.Id == cartItemId);

            if (cartItem == null)
                return false;

            if (quantity <= 0)
            {
                _context.ShoppingCartItems.Remove(cartItem);
            }
            else if (quantity <= cartItem.Product.StockQuantity)
            {
                cartItem.Quantity = quantity;
            }
            else
            {
                return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        try
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ClearCartAsync(string userId)
    {
        try
        {
            var cartItems = await _context.ShoppingCartItems
                .Where(sci => sci.UserId == userId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<decimal> GetCartTotalAsync(string userId)
    {
        var cartItems = await _context.ShoppingCartItems
            .Include(sci => sci.Product)
            .Where(sci => sci.UserId == userId)
            .ToListAsync();

        return cartItems.Sum(item => item.Product.Price * item.Quantity);
    }

    public async Task<int> GetCartItemCountAsync(string userId)
    {
        return await _context.ShoppingCartItems
            .Where(sci => sci.UserId == userId)
            .SumAsync(sci => sci.Quantity);
    }
}