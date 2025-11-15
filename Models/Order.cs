using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    
    [Required]
    [StringLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string DeliveryMethod { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveryPrice { get; set; } = 0;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.New;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    New,      
    Processing, 
    Shipped,    
    Delivered, 
    Cancelled  
}