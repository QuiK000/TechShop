using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Прізвище обов'язкове")]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обов'язковий")]
    [Phone(ErrorMessage = "Некоректний номер телефону")]
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Адреса")]
    public string? Address { get; set; }
}