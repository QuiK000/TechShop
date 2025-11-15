using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Прізвище обов'язкове")]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Некоректний email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обов'язковий")]
    [Phone(ErrorMessage = "Некоректний номер телефону")]
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути мінімум 6 символів")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; } = string.Empty;
}