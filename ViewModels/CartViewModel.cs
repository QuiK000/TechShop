namespace WebApplication2.ViewModels;

public class CartViewModel
{
    public IEnumerable<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    public decimal TotalAmount { get; set; }
    public decimal DeliveryPrice { get; set; }
    public decimal GrandTotal { get; set; }
}