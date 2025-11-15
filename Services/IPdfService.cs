using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IPdfService
{
    Task<byte[]> GenerateOrderReceiptAsync(Order order);
}