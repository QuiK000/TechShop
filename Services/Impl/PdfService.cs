using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebApplication2.Models;

namespace WebApplication2.Services.Impl;

public class PdfService : IPdfService
{
    public async Task<byte[]> GenerateOrderReceiptAsync(Order order)
    {
        return await Task.Run(() =>
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Header()
                        .Height(40)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(5)
                        .Column(column =>
                        {
                            column.Spacing(1);
                            
                            column.Item().Text("МАГАЗИН КОМП'ЮТЕРНОЇ ТЕХНІКИ")
                                .FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
                            
                            column.Item().Text("м. Хмельницький | Тел: +380 (38) 234-56-78")
                                .FontSize(6);
                        });

                    // КОНТЕНТ
                    page.Content()
                        .PaddingVertical(5)
                        .Column(column =>
                        {
                            column.Spacing(5);
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingBottom(3)
                                .Column(col =>
                                {
                                    col.Item().Text($"ЧЕК №{order.OrderNumber}").FontSize(10).Bold();
                                    col.Item().Text($"Дата: {order.CreatedAt:dd.MM.yyyy HH:mm} | Статус: {GetStatusText(order.Status)}");
                                });
                            
                            column.Item().Background(Colors.Grey.Lighten4).Padding(5)
                                .Column(col =>
                                {
                                    col.Item().Text("ІНФОРМАЦІЯ ПРО КЛІЄНТА").Bold().FontSize(8);
                                    col.Item().Text($"ПІБ: {order.CustomerName}");
                                    col.Item().Text($"Телефон: {order.CustomerPhone}");
                                    col.Item().Text($"Email: {order.CustomerEmail}");
                                    col.Item().Text($"Адреса: {order.DeliveryAddress}");
                                });
                            
                            column.Item().Text("ТОВАРИ").Bold().FontSize(8);
                            column.Item().PaddingTop(2).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(18);  // №
                                    columns.RelativeColumn(2);   // Назва товару
                                    columns.ConstantColumn(30);  // К-сть
                                    columns.ConstantColumn(50);  // Ціна
                                    columns.ConstantColumn(50);  // Сума
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(1).Text("№").Bold().FontSize(7);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(1).Text("Назва").Bold().FontSize(7);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(1).Text("К-сть").Bold().FontSize(7);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(1).Text("Ціна").Bold().FontSize(7);
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(1).Text("Сума").Bold().FontSize(7);
                                });

                                int index = 1;
                                foreach (var item in order.OrderItems)
                                {
                                    var bgColor = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                    table.Cell().Background(bgColor).Padding(1).Text(index.ToString()).FontSize(7);
                                    table.Cell().Background(bgColor).Padding(1).Text(item.ProductName).FontSize(7);
                                    table.Cell().Background(bgColor).Padding(1).Text(item.Quantity.ToString()).FontSize(7);
                                    table.Cell().Background(bgColor).Padding(1).Text($"{item.Price:N2} грн").FontSize(7);
                                    table.Cell().Background(bgColor).Padding(1).Text($"{item.TotalPrice:N2} грн").FontSize(7);

                                    index++;
                                }
                            });

                            // Підсумки
                            column.Item().AlignRight().Column(col =>
                            {
                                var subtotal = order.TotalAmount - order.DeliveryPrice;

                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Вартість товарів:").FontSize(8);
                                    row.ConstantItem(60).AlignRight().Text($"{subtotal:N2} грн").FontSize(8);
                                });

                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Доставка:").FontSize(8);
                                    row.ConstantItem(60).AlignRight().Text($"{order.DeliveryPrice:N2} грн").FontSize(8);
                                });

                                col.Item().BorderTop(1).BorderColor(Colors.Black)
                                    .PaddingTop(2).Row(row =>
                                    {
                                        row.RelativeItem().Text("РАЗОМ:").Bold().FontSize(9);
                                        row.ConstantItem(60).AlignRight().Text($"{order.TotalAmount:N2} грн")
                                            .Bold().FontSize(9).FontColor(Colors.Blue.Darken2);
                                    });
                            });

                            // Додаткова інформація
                            column.Item().Background(Colors.Grey.Lighten4).Padding(5)
                                .Column(col =>
                                {
                                    col.Item().Text($"Оплата: {order.PaymentMethod}").FontSize(8);
                                    col.Item().Text($"Доставка: {order.DeliveryMethod}").FontSize(8);
                                    if (!string.IsNullOrEmpty(order.Notes))
                                    {
                                        col.Item().Text($"Коментар: {order.Notes}").FontSize(8);
                                    }
                                });
                        });

                    // Футер
                    page.Footer()
                        .AlignCenter()
                        .PaddingBottom(5)
                        .Text("Дякуємо за покупку! | www.techshop.ua")
                        .FontSize(7);
                });
            });

            return document.GeneratePdf();
        });
    }

    private string GetStatusText(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.New => "Нове",
            OrderStatus.Processing => "В обробці",
            OrderStatus.Shipped => "Відправлено",
            OrderStatus.Delivered => "Доставлено",
            OrderStatus.Cancelled => "Скасовано",
            _ => status.ToString()
        };
    }
}