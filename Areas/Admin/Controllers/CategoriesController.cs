using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    // GET: Admin/Categories/Create
    public async Task<IActionResult> Create()
    {
        var category = new Category();
        var parentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();

        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");

        return View(category);
    }

    // POST: Admin/Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        Console.WriteLine("=== CREATE CATEGORY STARTED ===");
        Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
        Console.WriteLine($"Category Name: {category.Name}");
        Console.WriteLine($"Category Description: {category.Description}");
        Console.WriteLine($"Category ParentCategoryId: {category.ParentCategoryId}");

        // Логуємо всі помилки валідації
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                foreach (var err in error.Value.Errors)
                {
                    Console.WriteLine($"Error in {error.Key}: {err.ErrorMessage}");
                }
            }

            var parentCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();

            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");
            return View(category);
        }

        try
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Категорію успішно створено!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            TempData["Error"] = $"Помилка при створенні категорії: {ex.Message}";

            var parentCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();

            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");
            return View(category);
        }
    }

    // GET: Admin/Categories/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            TempData["Error"] = "Категорію не знайдено";
            return RedirectToAction(nameof(Index));
        }

        var parentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null && c.Id != id)
            .ToListAsync();

        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");

        return View(category);
    }

    // POST: Admin/Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        Console.WriteLine("=== EDIT CATEGORY STARTED ===");
        Console.WriteLine($"Category ID: {category.Id}");
        Console.WriteLine($"Received Name: '{category.Name}'");
        Console.WriteLine($"Received Description: '{category.Description}'");
        Console.WriteLine($"Received ParentCategoryId: {category.ParentCategoryId}");
        Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

        ModelState.Remove("Products");
        ModelState.Remove("SubCategories");
        ModelState.Remove("ParentCategory");

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                foreach (var err in error.Value.Errors)
                {
                    Console.WriteLine($"Error in {error.Key}: {err.ErrorMessage}");
                }
            }

            var parentCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.Id != category.Id)
                .ToListAsync();

            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");
            return View(category);
        }

        try
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
            {
                TempData["Error"] = "Категорію не знайдено";
                return RedirectToAction(nameof(Index));
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ParentCategoryId = category.ParentCategoryId;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Категорію успішно оновлено!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(category.Id))
            {
                TempData["Error"] = "Категорію не знайдено";
                return RedirectToAction(nameof(Index));
            }

            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            TempData["Error"] = $"Помилка при оновленні категорії: {ex.Message}";

            var parentCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.Id != category.Id)
                .ToListAsync();

            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");
            return View(category);
        }
    }

    // POST: Admin/Categories/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return Json(new { success = false, message = "Категорію не знайдено" });

        if (category.Products.Any())
            return Json(new { success = false, message = "Неможливо видалити категорію з товарами" });

        if (category.SubCategories.Any())
            return Json(new { success = false, message = "Неможливо видалити категорію з підкатегоріями" });

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Категорію видалено" });
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}