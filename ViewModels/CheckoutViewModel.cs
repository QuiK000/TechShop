using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "ПІБ обов'язкове")]
    [Display(Name = "ПІБ")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Некоректний email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обов'язковий")]
    [Phone(ErrorMessage = "Некоректний телефон")]
    [Display(Name = "Телефон")]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Адреса доставки обов'язкова")]
    [Display(Name = "Адреса доставки")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Оберіть спосіб доставки")]
    [Display(Name = "Спосіб доставки")]
    public string DeliveryMethod { get; set; } = string.Empty;

    [Required(ErrorMessage = "Оберіть спосіб оплати")]
    [Display(Name = "Спосіб оплати")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Display(Name = "Коментар до замовлення")]
    public string? Notes { get; set; }
}