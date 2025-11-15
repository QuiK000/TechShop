using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
    
    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
}