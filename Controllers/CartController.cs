using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers;

public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _cartService = cartService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var totalAmount = await _cartService.GetCartTotalAsync(userId);

            var deliveryPrice = _configuration.GetValue<decimal>("AppSettings:DeliveryPrice");
            var freeDeliveryMin = _configuration.GetValue<decimal>("AppSettings:FreeDeliveryMinAmount");

            if (totalAmount >= freeDeliveryMin)
            {
                deliveryPrice = 0;
            }

            var viewModel = new CartViewModel
            {
                Items = cartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.ImageUrl,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity
                }),
                TotalAmount = totalAmount,
                DeliveryPrice = deliveryPrice,
                GrandTotal = totalAmount + deliveryPrice
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Потрібно увійти в систему" });
            }

            var result = await _cartService.AddToCartAsync(userId, productId, quantity);

            if (result)
            {
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, message = "Товар додано до кошика", cartCount = itemCount });
            }

            return Json(new { success = false, message = "Не вдалося додати товар до кошика" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var result = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);

            if (result)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != null)
                {
                    var total = await _cartService.GetCartTotalAsync(userId);
                    return Json(new { success = true, total });
                }
            }

            return Json(new { success = false, message = "Не вдалося оновити кількість" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var result = await _cartService.RemoveFromCartAsync(cartItemId);

            if (result)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != null)
                {
                    var total = await _cartService.GetCartTotalAsync(userId);
                    var itemCount = await _cartService.GetCartItemCountAsync(userId);
                    return Json(new { success = true, total, cartCount = itemCount });
                }
            }

            return Json(new { success = false, message = "Не вдалося видалити товар" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Користувача не знайдено" });
            }

            var result = await _cartService.ClearCartAsync(userId);

            if (result)
            {
                return Json(new { success = true, message = "Кошик очищено" });
            }

            return Json(new { success = false, message = "Не вдалося очистити кошик" });
        }

        [Authorize]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var count = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { count });
        }
    }