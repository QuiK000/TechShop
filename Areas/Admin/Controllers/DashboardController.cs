using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services;

namespace WebApplication2.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class DashboardController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public DashboardController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _statisticsService.GetDashboardDataAsync();
        return View(dashboardData);
    }

    public async Task<IActionResult> Statistics(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var salesData = await _statisticsService.GetSalesDataAsync(fromDate, toDate);
        var categorySales = await _statisticsService.GetCategorySalesAsync();
        var topProducts = await _statisticsService.GetTopProductsAsync(10);

        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.SalesData = salesData;
        ViewBag.CategorySales = categorySales;
        ViewBag.TopProducts = topProducts;

        return View();
    }
}