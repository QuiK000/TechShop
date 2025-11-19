using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    // DASHBOARD
    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount),

            NewOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.New)
                .CountAsync(),

            LowStockProducts = await _context.Products
                .Where(p => p.StockQuantity < 10)
                .CountAsync(),

            RecentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync(),

            TopProducts = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key,
                    ProductName = g.First().ProductName,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(5)
                .ToListAsync(),

            RecentUsers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }

    // ПРОДУКТИ
    public async Task<IActionResult> Products(string search, int? categoryId, int page = 1)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var pageSize = 20;
        var totalItems = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product product, IFormFile? image)
    {
        try
        {
            // Логування отриманих даних
            Console.WriteLine("===== CreateProduct started =====");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Price: {product.Price}");
            Console.WriteLine($"CategoryId: {product.CategoryId}");
            Console.WriteLine($"Brand: {product.Brand}");
            Console.WriteLine($"StockQuantity: {product.StockQuantity}");
            Console.WriteLine($"IsAvailable: {product.IsAvailable}");

            // Видаляємо з ModelState поля, які не приходять з форми
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            ModelState.Remove("OrderItems");
            ModelState.Remove("Reviews");
            ModelState.Remove("ShoppingCartItems");
            ModelState.Remove("WishlistItems");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("AverageRating");
            ModelState.Remove("ReviewCount");
            ModelState.Remove("image");

            // Логування стану ModelState
            Console.WriteLine($"ModelState valid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                TempData["Error"] = "Помилка валідації форми. Перевірте введені дані.";
                return View(product);
            }

            // Обробка зображення
            if (image != null && image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    TempData["Error"] = "Недозволений формат файлу. Використовуйте JPG, PNG, GIF або WebP.";
                    return View(product);
                }

                if (image.Length > 5 * 1024 * 1024) // 5MB
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    TempData["Error"] = "Розмір файлу не повинен перевищувати 5MB.";
                    return View(product);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");

                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                product.ImageUrl = $"/images/products/{fileName}";
            }
            else
            {
                product.ImageUrl = "/images/no-image.jpg";
            }

            // Встановлюємо значення за замовчуванням
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.AverageRating = 0;
            product.ReviewCount = 0;

            // Перевірка існування категорії
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                TempData["Error"] = "Обрана категорія не існує.";
                return View(product);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Product created successfully with ID: {product.Id}");
            TempData["Success"] = "Товар успішно створено!";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating product: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            ViewBag.Categories = await _context.Categories.ToListAsync();
            TempData["Error"] = $"Помилка при створенні товару: {ex.Message}";
            return View(product);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            TempData["Error"] = "Товар не знайдено";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Product product, IFormFile? image)
    {
        try
        {
            Console.WriteLine("===== EditProduct started =====");
            Console.WriteLine($"ID: {product.Id}");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Price: {product.Price}");

            // Видаляємо з ModelState поля, які не приходять з форми
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            ModelState.Remove("OrderItems");
            ModelState.Remove("Reviews");
            ModelState.Remove("ShoppingCartItems");
            ModelState.Remove("WishlistItems");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("AverageRating");
            ModelState.Remove("ReviewCount");
            ModelState.Remove("image");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }

                ViewBag.Categories = await _context.Categories.ToListAsync();
                TempData["Error"] = "Помилка валідації форми. Перевірте введені дані.";
                return View(product);
            }

            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                TempData["Error"] = "Товар не знайдено";
                return RedirectToAction(nameof(Products));
            }

            // Обробка зображення
            if (image != null && image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    TempData["Error"] = "Недозволений формат файлу. Використовуйте JPG, PNG, GIF або WebP.";
                    return View(product);
                }

                if (image.Length > 5 * 1024 * 1024)
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    TempData["Error"] = "Розмір файлу не повинен перевищувати 5MB.";
                    return View(product);
                }

                // Видалити старе зображення
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl) &&
                    existingProduct.ImageUrl != "/images/no-image.jpg")
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = $"/images/products/{fileName}";
            }

            // Оновлюємо поля
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Brand = product.Brand;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.Specifications = product.Specifications;
            existingProduct.IsAvailable = product.IsAvailable;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар успішно оновлено!";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product: {ex.Message}");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            TempData["Error"] = $"Помилка при оновленні товару: {ex.Message}";
            return View(product);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return Json(new { success = false, message = "Товар не знайдено" });

        // Видалити зображення
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var path = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Товар видалено" });
    }

    [HttpPost]
    public async Task<IActionResult> BulkAction(string action, List<int> ids)
    {
        if (ids == null || !ids.Any())
            return Json(new { success = false, message = "Не обрано товарів" });

        switch (action)
        {
            case "activate":
                await _context.Products
                    .Where(p => ids.Contains(p.Id))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsAvailable, true));
                break;

            case "deactivate":
                await _context.Products
                    .Where(p => ids.Contains(p.Id))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsAvailable, false));
                break;

            case "delete":
                var products = await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
                _context.Products.RemoveRange(products);
                await _context.SaveChangesAsync();
                break;

            default:
                return Json(new { success = false, message = "Невідома дія" });
        }

        return Json(new { success = true, message = "Дію виконано успішно" });
    }

    // ЗАМОВЛЕННЯ
    public async Task<IActionResult> Orders(OrderStatus? status, DateTime? from, DateTime? to, int page = 1)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var pageSize = 20;
        var totalItems = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.Status = status;
        ViewBag.From = from;
        ViewBag.To = to;

        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return Json(new { success = false, message = "Замовлення не знайдено" });

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Статус оновлено" });
    }

    // СТАТИСТИКА
    // СТАТИСТИКА
    public async Task<IActionResult> Analytics(DateTime? from, DateTime? to)
    {
        try
        {
            var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = to ?? DateTime.UtcNow;

            // Переконуємося, що toDate включає весь день
            toDate = toDate.Date.AddDays(1).AddSeconds(-1);

            Console.WriteLine($"Analytics: from {fromDate} to {toDate}");

            var model = new AnalyticsViewModel
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            // Статистика продажів по дням
            var salesDataQuery = _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .AsEnumerable() // Переходимо до локального виконання для группування по даті
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesDataPoint
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(s => s.Date)
                .ToList();

            model.SalesData = salesDataQuery;

            Console.WriteLine($"Found {salesDataQuery.Count} days with sales data");

            // Продажі по категоріях
            var categorySalesQuery = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
                .ToListAsync();

            var categorySales = categorySalesQuery
                .GroupBy(oi => oi.Product.Category?.Name ?? "Без категорії")
                .Select(g => new CategorySalesDto
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(oi => oi.Price * oi.Quantity),
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(c => c.TotalSales)
                .ToList();

            model.CategorySales = categorySales;

            Console.WriteLine($"Found {categorySales.Count} categories with sales");

            // Топ клієнти
            var topCustomersQuery = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.User != null)
                .ToListAsync();

            var topCustomers = topCustomersQuery
                .GroupBy(o => o.UserId)
                .Select(g => new TopCustomerDto
                {
                    UserId = g.Key,
                    CustomerName = !string.IsNullOrEmpty(g.First().User.FirstName) &&
                                   !string.IsNullOrEmpty(g.First().User.LastName)
                        ? $"{g.First().User.FirstName} {g.First().User.LastName}"
                        : g.First().User.Email ?? "Анонімний клієнт",
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            model.TopCustomers = topCustomers;

            Console.WriteLine($"Found {topCustomers.Count} top customers");

            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Analytics: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Повертаємо пусту модель у випадку помилки
            var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = to ?? DateTime.UtcNow;

            return View(new AnalyticsViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                SalesData = new List<SalesDataPoint>(),
                CategorySales = new List<CategorySalesDto>(),
                TopCustomers = new List<TopCustomerDto>()
            });
        }
    }
}

// ViewModels
public class AdminDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewOrders { get; set; }
    public int LowStockProducts { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
    public List<ApplicationUser> RecentUsers { get; set; } = new();
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}

public class AnalyticsViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<SalesDataPoint> SalesData { get; set; } = new();
    public List<CategorySalesDto> CategorySales { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class SalesDataPoint
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

public class CategorySalesDto
{
    public string CategoryName { get; set; }
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
}

public class TopCustomerDto
{
    public string UserId { get; set; }
    public string CustomerName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}