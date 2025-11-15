using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPdfService _pdfService;

    public OrdersController(IOrderService orderService, IPdfService pdfService)
    {
        _orderService = orderService;
        _pdfService = pdfService;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, status);

        if (result)
        {
            TempData["Success"] = "Статус замовлення оновлено!";
            return Json(new { success = true, message = "Статус оновлено" });
        }

        return Json(new { success = false, message = "Не вдалося оновити статус" });
    }

    public async Task<IActionResult> DownloadReceipt(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var pdfBytes = await _pdfService.GenerateOrderReceiptAsync(order);
        return File(pdfBytes, "application/pdf", $"Receipt_{order.OrderNumber}.pdf");
    }
}