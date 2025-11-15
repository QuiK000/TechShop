using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Services.Impl;

public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _context;

    public StatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);
        var monthAgo = now.AddMonths(-1);

        var totalOrders = await _context.Orders.CountAsync();
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var weekRevenue = await _context.Orders
            .Where(o => o.CreatedAt >= weekAgo && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var monthRevenue = await _context.Orders
            .Where(o => o.CreatedAt >= monthAgo && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var totalUsers = await _context.Users.CountAsync();
        var totalProducts = await _context.Products.CountAsync();

        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .ToListAsync();

        var topProducts = await GetTopProductsAsync(10);

        return new DashboardViewModel
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            MonthRevenue = monthRevenue,
            WeekRevenue = weekRevenue,
            TotalUsers = totalUsers,
            TotalProducts = totalProducts,
            RecentOrders = recentOrders,
            TopProducts = topProducts
        };
    }

    public async Task<IEnumerable<SalesDataPoint>> GetSalesDataAsync(DateTime from, DateTime to)
    {
        var orders = await _context.Orders
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to && o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new SalesDataPoint
            {
                Date = g.Key,
                Amount = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        return orders;
    }

    public async Task<IEnumerable<CategorySalesData>> GetCategorySalesAsync()
    {
        var categorySales = await _context.OrderItems
            .Include(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => oi.Product.Category.Name)
            .Select(g => new CategorySalesData
            {
                CategoryName = g.Key,
                TotalSales = g.Sum(oi => oi.TotalPrice),
                OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderByDescending(c => c.TotalSales)
            .ToListAsync();

        return categorySales;
    }

    public async Task<IEnumerable<TopProductData>> GetTopProductsAsync(int count)
    {
        var topProducts = await _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopProductData
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Price * oi.Quantity) // Обчислюємо на льоту
            })
            .OrderByDescending(p => p.Revenue)
            .Take(count)
            .ToListAsync();

        return topProducts;
    }
}