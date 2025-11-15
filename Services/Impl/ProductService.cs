using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Services.Impl;

public class ProductService(ApplicationDbContext context) : IProductService
{
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsAvailable)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllProductsAsync();

        var term = searchTerm.ToLower();
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.IsAvailable &&
                        (p.Name.ToLower().Contains(term) ||
                         p.Description.ToLower().Contains(term) ||
                         p.Brand!.ToLower().Contains(term)))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count)
    {
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.IsAvailable)
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        try
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            product.UpdatedAt = DateTime.UtcNow;
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return false;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task UpdateProductRatingAsync(int productId)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null)
            return;

        var reviews = await context.Reviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .ToListAsync();

        if (reviews.Any())
        {
            product.AverageRating = (decimal)reviews.Average(r => r.Rating);
            product.ReviewCount = reviews.Count;
        }
        else
        {
            product.AverageRating = 0;
            product.ReviewCount = 0;
        }

        await context.SaveChangesAsync();
    }
}