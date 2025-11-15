namespace WebApplication2.ViewModels;

public class CategorySalesData
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
}