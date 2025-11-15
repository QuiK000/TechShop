using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<bool> CanUserReviewProductAsync(string userId, int productId);
    Task<bool> CreateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int id);
    Task<bool> ApproveReviewAsync(int id);
    Task<bool> AddAdminResponseAsync(int reviewId, string response);
}