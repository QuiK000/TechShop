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

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.ParentCategories = new SelectList(
            await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync(), 
            "Id", "Name"
        );
        return View();
    }

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

        ViewBag.ParentCategories = new SelectList(
            await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync(), 
            "Id", "Name"
        );
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        ViewBag.ParentCategories = new SelectList(
            await _context.Categories.Where(c => c.ParentCategoryId == null && c.Id != id).ToListAsync(), 
            "Id", "Name", category.ParentCategoryId
        );
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Категорію успішно оновлено!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(category.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        ViewBag.ParentCategories = new SelectList(
            await _context.Categories.Where(c => c.ParentCategoryId == null && c.Id != id).ToListAsync(), 
            "Id", "Name", category.ParentCategoryId
        );
        return View(category);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        if (category.Products.Any())
        {
            TempData["Error"] = "Неможливо видалити категорію, яка містить товари!";
            return RedirectToAction(nameof(Index));
        }

        if (category.SubCategories.Any())
        {
            TempData["Error"] = "Неможливо видалити категорію, яка містить підкатегорії!";
            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Категорію успішно видалено!";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> CategoryExists(int id)
    {
        return await _context.Categories.AnyAsync(e => e.Id == id);
    }
}