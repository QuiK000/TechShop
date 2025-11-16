using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index(string search, int page = 1)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search) ||
                u.Email.Contains(search));
        }

        var pageSize = 20;
        var totalItems = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Додаємо ролі для кожного користувача
        var usersWithRoles = new List<UserWithRolesViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var orderCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);
            
            usersWithRoles.Add(new UserWithRolesViewModel
            {
                User = user,
                Roles = roles.ToList(),
                OrderCount = orderCount,
                IsLocked = await _userManager.IsLockedOutAsync(user)
            });
        }

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.Search = search;

        return View(usersWithRoles);
    }

    // GET: Admin/Users/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var orders = await _context.Orders
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var model = new UserDetailsViewModel
        {
            User = user,
            Roles = roles.ToList(),
            Orders = orders,
            IsLocked = await _userManager.IsLockedOutAsync(user)
        };

        return View(model);
    }

    // GET: Admin/Users/EditRoles/5
    [HttpGet]
    public async Task<IActionResult> EditRoles(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        var model = new EditRolesViewModel
        {
            UserId = user.Id,
            UserName = $"{user.FirstName} {user.LastName}",
            UserEmail = user.Email,
            AllRoles = allRoles.Select(r => new RoleSelectionViewModel
            {
                RoleName = r.Name!,
                IsSelected = userRoles.Contains(r.Name!)
            }).ToList()
        };

        return View(model);
    }

    // POST: Admin/Users/EditRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoles(EditRolesViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.AllRoles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

        // Видалити всі поточні ролі
        var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
        if (!removeResult.Succeeded)
        {
            ModelState.AddModelError("", "Не вдалося видалити поточні ролі");
            return View(model);
        }

        // Додати нові ролі
        if (selectedRoles.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Не вдалося додати нові ролі");
                return View(model);
            }
        }

        TempData["Success"] = "Ролі користувача оновлено!";
        return RedirectToAction(nameof(Details), new { id = user.Id });
    }

    // POST: Admin/Users/Lock/5
    [HttpPost]
    public async Task<IActionResult> Lock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Json(new { success = false, message = "Користувача не знайдено" });
        }

        // Не можна заблокувати себе
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == id)
        {
            return Json(new { success = false, message = "Ви не можете заблокувати себе" });
        }

        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Користувача заблоковано" });
        }

        return Json(new { success = false, message = "Помилка блокування" });
    }

    // POST: Admin/Users/Unlock/5
    [HttpPost]
    public async Task<IActionResult> Unlock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Json(new { success = false, message = "Користувача не знайдено" });
        }

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Користувача розблоковано" });
        }

        return Json(new { success = false, message = "Помилка розблокування" });
    }

    // POST: Admin/Users/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Json(new { success = false, message = "Користувача не знайдено" });
        }

        // Не можна видалити себе
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == id)
        {
            return Json(new { success = false, message = "Ви не можете видалити себе" });
        }

        // Перевірка чи є замовлення
        var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
        if (hasOrders)
        {
            return Json(new { success = false, message = "Неможливо видалити користувача з замовленнями" });
        }

        var result = await _userManager.DeleteAsync(user);
        
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Користувача видалено" });
        }

        return Json(new { success = false, message = "Помилка видалення" });
    }
}

// ViewModels
public class UserWithRolesViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public int OrderCount { get; set; }
    public bool IsLocked { get; set; }
}

public class UserDetailsViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public bool IsLocked { get; set; }
}

public class EditRolesViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public List<RoleSelectionViewModel> AllRoles { get; set; } = new();
}

public class RoleSelectionViewModel
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}