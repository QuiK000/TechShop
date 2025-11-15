using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Services.Impl;

public class ReviewService(ApplicationDbContext context, IProductService productService)
    : IReviewService
{
    public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> CanUserReviewProductAsync(string userId, int productId)
    {
        var existingReview = await context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);

        if (existingReview != null)
            return false;
        
        var hasPurchased = await context.OrderItems
            .Include(oi => oi.Order)
            .AnyAsync(oi => oi.ProductId == productId &&
                            oi.Order.UserId == userId &&
                            oi.Order.Status == OrderStatus.Delivered);

        return hasPurchased;
    }

    public async Task<bool> CreateReviewAsync(Review review)
    {
        try
        {
            if (!await CanUserReviewProductAsync(review.UserId, review.ProductId))
                return false;

            review.CreatedAt = DateTime.UtcNow;
            review.IsApproved = true;

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
            
            await productService.UpdateProductRatingAsync(review.ProductId);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        try
        {
            var review = await context.Reviews.FindAsync(id);
            if (review == null)
                return false;

            var productId = review.ProductId;

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            await productService.UpdateProductRatingAsync(productId);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ApproveReviewAsync(int id)
    {
        try
        {
            var review = await context.Reviews.FindAsync(id);
            if (review == null)
                return false;

            review.IsApproved = true;
            await context.SaveChangesAsync();
            await productService.UpdateProductRatingAsync(review.ProductId);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddAdminResponseAsync(int reviewId, string response)
    {
        try
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            review.AdminResponse = response;
            review.AdminResponseDate = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}