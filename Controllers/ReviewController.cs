using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers;

[Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Заповніть всі поля відгуку";
                return RedirectToAction("Details", "Product", new { id = model.ProductId });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var canReview = await _reviewService.CanUserReviewProductAsync(userId, model.ProductId);
            if (!canReview)
            {
                TempData["Error"] = "Ви не можете залишити відгук на цей товар";
                return RedirectToAction("Details", "Product", new { id = model.ProductId });
            }

            var review = new Review
            {
                ProductId = model.ProductId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            var result = await _reviewService.CreateReviewAsync(review);

            if (result)
            {
                TempData["Success"] = "Дякуємо за відгук!";
            }
            else
            {
                TempData["Error"] = "Не вдалося додати відгук";
            }

            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);

            if (result)
            {
                return Json(new { success = true, message = "Відгук видалено" });
            }

            return Json(new { success = false, message = "Не вдалося видалити відгук" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddResponse(int reviewId, string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return Json(new { success = false, message = "Відповідь не може бути порожньою" });
            }

            var result = await _reviewService.AddAdminResponseAsync(reviewId, response);

            if (result)
            {
                return Json(new { success = true, message = "Відповідь додано" });
            }

            return Json(new { success = false, message = "Не вдалося додати відповідь" });
        }
    }