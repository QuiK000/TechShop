using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        ViewBag.ParentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
        return View();
    }

    // POST: Admin/Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Категорію успішно створено!";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.ParentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
        return View(category);
    }

    // GET: Admin/Categories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        ViewBag.ParentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null && c.Id != id)
            .ToListAsync();

        return View(category);
    }

    // POST: Admin/Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Категорію оновлено!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                    return NotFound();
                throw;
            }
        }

        ViewBag.ParentCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == null && c.Id != id)
            .ToListAsync();

        return View(category);
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