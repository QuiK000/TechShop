using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Comment { get; set; } = string.Empty;

    public bool IsApproved { get; set; } = true; // Модерація

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? AdminResponse { get; set; }
    public DateTime? AdminResponseDate { get; set; }
}