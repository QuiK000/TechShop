namespace WebApplication2.Services;

public interface IExportService
{
    Task<byte[]> ExportOrdersToExcelAsync(DateTime from, DateTime to);
    Task<byte[]> ExportProductsToExcelAsync();
    Task<byte[]> ExportCustomersToExcelAsync();
}