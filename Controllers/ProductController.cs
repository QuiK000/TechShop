using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Services;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers;

public class ProductController(
    ApplicationDbContext context,
    IProductService productService,
    IReviewService reviewService)
    : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, decimal? minPrice, 
            decimal? maxPrice, string? brand, string sortBy = "name", int pageNumber = 1, int pageSize = 12)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) || 
                                        p.Description.ToLower().Contains(term) ||
                                        p.Brand!.ToLower().Contains(term));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            // Сортування
            query = sortBy.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating" => query.OrderByDescending(p => p.AverageRating),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await context.Categories.ToListAsync();

            var viewModel = new ProductCatalogViewModel
            {
                Products = products,
                Categories = categories,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Brand = brand,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var reviews = await reviewService.GetProductReviewsAsync(id);
            var canReview = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    canReview = await reviewService.CanUserReviewProductAsync(userId, id);
                }
            }

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Reviews = reviews,
                CanReview = canReview
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Category(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var products = await productService.GetProductsByCategoryAsync(id);

            ViewBag.CategoryName = category.Name;
            return View(products);
        }
    }