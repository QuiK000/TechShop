using WebApplication2.Models;

namespace WebApplication2.ViewModels;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
    public bool CanReview { get; set; }
    public ReviewViewModel? NewReview { get; set; }
}