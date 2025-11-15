using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> GetOrderByNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<string> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task<decimal> CalculateTotalAmountAsync(List<OrderItem> items);
}