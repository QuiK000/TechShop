using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

public class HomeController(ApplicationDbContext context, IProductService productService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var featuredProducts = await productService.GetFeaturedProductsAsync(8);
        var categories = await context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.Categories = categories;

        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}