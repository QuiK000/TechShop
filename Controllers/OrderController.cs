using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers;

[Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IPdfService _pdfService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            IPdfService pdfService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _cartService = cartService;
            _pdfService = pdfService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Кошик порожній!";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                CustomerName = $"{user?.FirstName} {user?.LastName}",
                CustomerEmail = user?.Email ?? "",
                CustomerPhone = user?.PhoneNumber ?? "",
                DeliveryAddress = user?.Address ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Кошик порожній!";
                return RedirectToAction("Index", "Cart");
            }

            var deliveryPrice = _configuration.GetValue<decimal>("AppSettings:DeliveryPrice");
            var freeDeliveryMin = _configuration.GetValue<decimal>("AppSettings:FreeDeliveryMinAmount");
            var totalAmount = await _cartService.GetCartTotalAsync(userId);

            if (totalAmount >= freeDeliveryMin)
            {
                deliveryPrice = 0;
            }

            var order = new Order
            {
                UserId = userId,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                DeliveryAddress = model.DeliveryAddress,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes,
                TotalAmount = totalAmount + deliveryPrice,
                DeliveryPrice = deliveryPrice,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList()
            };

            try
            {
                var orderNumber = await _orderService.CreateOrderAsync(order);
                await _cartService.ClearCartAsync(userId);

                TempData["Success"] = $"Замовлення №{orderNumber} успішно оформлено!";
                return RedirectToAction("OrderConfirmation", new { orderNumber });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при оформленні замовлення: {ex.Message}";
                return View(model);
            }
        }

        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        public async Task<IActionResult> DownloadReceipt(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (order.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var pdfBytes = await _pdfService.GenerateOrderReceiptAsync(order);
            return File(pdfBytes, "application/pdf", $"Receipt_{order.OrderNumber}.pdf");
        }
    }