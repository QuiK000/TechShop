using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class ShoppingCartItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

public class WishlistItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}