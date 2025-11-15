using WebApplication2.Models;

namespace WebApplication2.ViewModels;

public class DashboardViewModel
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public decimal WeekRevenue { get; set; }
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
    public IEnumerable<TopProductData> TopProducts { get; set; } = new List<TopProductData>();
}
