using WebApplication2.Models;

namespace WebApplication2.Services;

public interface ICartService
{
    Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string userId);
    Task<bool> AddToCartAsync(string userId, int productId, int quantity);
    Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
    Task<bool> RemoveFromCartAsync(int cartItemId);
    Task<bool> ClearCartAsync(string userId);
    Task<decimal> GetCartTotalAsync(string userId);
    Task<int> GetCartItemCountAsync(string userId);
}