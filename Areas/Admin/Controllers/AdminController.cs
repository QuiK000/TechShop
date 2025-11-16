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
                    Revenue = g.Sum(oi => oi.Price * oi.Quantity) // ✅ Виправлено
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
    public async Task<IActionResult> CreateProduct(Product product, IFormFile image)
    {
        if (ModelState.IsValid)
        {
            if (image != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine(_environment.WebRootPath, "images", "products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var stream = new FileStream(path, FileMode.Create))
                    await image.CopyToAsync(stream);

                product.ImageUrl = $"/images/products/{fileName}";
            }

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар успішно створено!";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(Product product, IFormFile image)
    {
        if (ModelState.IsValid)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null) return NotFound();

            if (image != null)
            {
                // Видалити старе зображення
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, existing.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine(_environment.WebRootPath, "images", "products", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await image.CopyToAsync(stream);

                existing.ImageUrl = $"/images/products/{fileName}";
            }

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            existing.Brand = product.Brand;
            existing.StockQuantity = product.StockQuantity;
            existing.Specifications = product.Specifications;
            existing.IsAvailable = product.IsAvailable;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар оновлено!";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
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

    // КОРИСТУВАЧІ
    public async Task<IActionResult> Users(string search, int page = 1)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u =>
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search) ||
                u.Email.Contains(search));

        var pageSize = 20;
        var totalItems = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.Search = search;

        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> LockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Json(new { success = false, message = "Користувача не знайдено" });

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        return Json(new { success = true, message = "Користувача заблоковано" });
    }

    [HttpPost]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Json(new { success = false, message = "Користувача не знайдено" });

        await _userManager.SetLockoutEndDateAsync(user, null);
        return Json(new { success = true, message = "Користувача розблоковано" });
    }

    // СТАТИСТИКА
    public async Task<IActionResult> Analytics(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var model = new AnalyticsViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,

            SalesData = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesDataPoint
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(s => s.Date)
                .ToListAsync(),

            CategorySales = await _context.OrderItems
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
                .GroupBy(oi => oi.Product.Category.Name)
                .Select(g => new CategorySalesDto
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(oi => oi.TotalPrice),
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(c => c.TotalSales)
                .ToListAsync(),

            TopCustomers = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.User != null)
                .GroupBy(o => o.UserId)
                .Select(g => new TopCustomerDto
                {
                    UserId = g.Key,
                    CustomerName = g.First().User.FirstName + " " + g.First().User.LastName,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToListAsync()
        };

        return View(model);
    }

    // НАЛАШТУВАННЯ
    public IActionResult Settings()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ClearCache()
    {
        // Логіка очищення кешу
        return Json(new { success = true, message = "Кеш очищено" });
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