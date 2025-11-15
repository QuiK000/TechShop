using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels;

public class ReviewViewModel
{
    public int ProductId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Оберіть рейтинг від 1 до 5")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Коментар обов'язковий")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Коментар має бути від 10 до 2000 символів")]
    public string Comment { get; set; } = string.Empty;
}