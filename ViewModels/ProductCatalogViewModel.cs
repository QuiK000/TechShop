using WebApplication2.Models;

namespace WebApplication2.ViewModels;

public class ProductCatalogViewModel
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Brand { get; set; }
    public string SortBy { get; set; } = "name";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
}