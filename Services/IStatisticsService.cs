using WebApplication2.ViewModels;

namespace WebApplication2.Services;

public interface IStatisticsService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<IEnumerable<SalesDataPoint>> GetSalesDataAsync(DateTime from, DateTime to);
    Task<IEnumerable<CategorySalesData>> GetCategorySalesAsync();
    Task<IEnumerable<TopProductData>> GetTopProductsAsync(int count);
}