using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;

namespace WebApplication2.Services.Impl;

public class ExportService : IExportService
{
    private readonly ApplicationDbContext _context;

    public ExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportOrdersToExcelAsync(DateTime from, DateTime to)
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Замовлення");

        // Headers
        worksheet.Cell(1, 1).Value = "№ Замовлення";
        worksheet.Cell(1, 2).Value = "Дата";
        worksheet.Cell(1, 3).Value = "Клієнт";
        worksheet.Cell(1, 4).Value = "Email";
        worksheet.Cell(1, 5).Value = "Телефон";
        worksheet.Cell(1, 6).Value = "Адреса";
        worksheet.Cell(1, 7).Value = "Доставка";
        worksheet.Cell(1, 8).Value = "Оплата";
        worksheet.Cell(1, 9).Value = "Сума";
        worksheet.Cell(1, 10).Value = "Статус";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data
        int row = 2;
        foreach (var order in orders)
        {
            worksheet.Cell(row, 1).Value = order.OrderNumber;
            worksheet.Cell(row, 2).Value = order.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            worksheet.Cell(row, 3).Value = order.CustomerName;
            worksheet.Cell(row, 4).Value = order.CustomerEmail;
            worksheet.Cell(row, 5).Value = order.CustomerPhone;
            worksheet.Cell(row, 6).Value = order.DeliveryAddress;
            worksheet.Cell(row, 7).Value = order.DeliveryMethod;
            worksheet.Cell(row, 8).Value = order.PaymentMethod;
            worksheet.Cell(row, 9).Value = order.TotalAmount;
            worksheet.Cell(row, 10).Value = GetStatusText(order.Status);
            row++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportProductsToExcelAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Товари");

        // Headers
        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Назва";
        worksheet.Cell(1, 3).Value = "Категорія";
        worksheet.Cell(1, 4).Value = "Бренд";
        worksheet.Cell(1, 5).Value = "Ціна";
        worksheet.Cell(1, 6).Value = "На складі";
        worksheet.Cell(1, 7).Value = "Рейтинг";
        worksheet.Cell(1, 8).Value = "Відгуків";
        worksheet.Cell(1, 9).Value = "Статус";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data
        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = product.Id;
            worksheet.Cell(row, 2).Value = product.Name;
            worksheet.Cell(row, 3).Value = product.Category.Name;
            worksheet.Cell(row, 4).Value = product.Brand;
            worksheet.Cell(row, 5).Value = product.Price;
            worksheet.Cell(row, 6).Value = product.StockQuantity;
            worksheet.Cell(row, 7).Value = product.AverageRating;
            worksheet.Cell(row, 8).Value = product.ReviewCount;
            worksheet.Cell(row, 9).Value = product.IsAvailable ? "Активний" : "Неактивний";
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportCustomersToExcelAsync()
    {
        var customers = await _context.Users
            .Include(u => u.Orders)
            .OrderBy(u => u.LastName)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Клієнти");

        // Headers
        worksheet.Cell(1, 1).Value = "Ім'я";
        worksheet.Cell(1, 2).Value = "Прізвище";
        worksheet.Cell(1, 3).Value = "Email";
        worksheet.Cell(1, 4).Value = "Телефон";
        worksheet.Cell(1, 5).Value = "Адреса";
        worksheet.Cell(1, 6).Value = "Замовлень";
        worksheet.Cell(1, 7).Value = "Дата реєстрації";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data
        int row = 2;
        foreach (var customer in customers)
        {
            worksheet.Cell(row, 1).Value = customer.FirstName;
            worksheet.Cell(row, 2).Value = customer.LastName;
            worksheet.Cell(row, 3).Value = customer.Email;
            worksheet.Cell(row, 4).Value = customer.PhoneNumber ?? "";
            worksheet.Cell(row, 5).Value = customer.Address ?? "";
            worksheet.Cell(row, 6).Value = customer.Orders.Count;
            worksheet.Cell(row, 7).Value = customer.CreatedAt.ToString("dd.MM.yyyy");
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private string GetStatusText(Models.OrderStatus status)
    {
        return status switch
        {
            Models.OrderStatus.New => "Нове",
            Models.OrderStatus.Processing => "В обробці",
            Models.OrderStatus.Shipped => "Відправлено",
            Models.OrderStatus.Delivered => "Доставлено",
            Models.OrderStatus.Cancelled => "Скасовано",
            _ => status.ToString()
        };
    }
}