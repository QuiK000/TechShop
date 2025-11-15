using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : Controller
{
    private readonly IExportService _exportService;
    private readonly IStatisticsService _statisticsService;

    public ReportsController(IExportService exportService, IStatisticsService statisticsService)
    {
        _exportService = exportService;
        _statisticsService = statisticsService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExportOrders(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var fileContent = await _exportService.ExportOrdersToExcelAsync(fromDate, toDate);
        var fileName = $"Orders_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx";

        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost]
    public async Task<IActionResult> ExportProducts()
    {
        var fileContent = await _exportService.ExportProductsToExcelAsync();
        var fileName = $"Products_{DateTime.Now:yyyyMMdd}.xlsx";

        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost]
    public async Task<IActionResult> ExportCustomers()
    {
        var fileContent = await _exportService.ExportCustomersToExcelAsync();
        var fileName = $"Customers_{DateTime.Now:yyyyMMdd}.xlsx";

        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}