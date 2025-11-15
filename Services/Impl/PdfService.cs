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
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().Text("МАГАЗИН КОМП'ЮТЕРНОЇ ТЕХНІКИ")
                                .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                            column.Item().Text("TechShop").FontSize(16).SemiBold();
                            column.Item().PaddingTop(5).Text("м. Київ, вул. Хрещатик, 1");
                            column.Item().Text("Телефон: +380 (44) 123-45-67 | Email: info@techshop.ua");
                        });

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Інформація про замовлення
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingBottom(10)
                                .Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text($"ЧЕК №{order.OrderNumber}").FontSize(16).Bold();
                                        col.Item().Text($"Дата: {order.CreatedAt:dd.MM.yyyy HH:mm}");
                                        col.Item().Text($"Статус: {GetStatusText(order.Status)}");
                                    });
                                });

                            column.Item().PaddingTop(20);

                            // Інформація про клієнта
                            column.Item().Background(Colors.Grey.Lighten4).Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("ІНФОРМАЦІЯ ПРО КЛІЄНТА").Bold().FontSize(12);
                                    col.Item().PaddingTop(5).Text($"ПІБ: {order.CustomerName}");
                                    col.Item().Text($"Телефон: {order.CustomerPhone}");
                                    col.Item().Text($"Email: {order.CustomerEmail}");
                                    col.Item().Text($"Адреса доставки: {order.DeliveryAddress}");
                                });

                            column.Item().PaddingTop(20);
                            column.Item().Text("ТОВАРИ").Bold().FontSize(12);
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30); // №
                                    columns.RelativeColumn(4); // Назва
                                    columns.RelativeColumn(1); // Кількість
                                    columns.RelativeColumn(1.5f); // Ціна
                                    columns.RelativeColumn(1.5f); // Сума
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("№").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Назва товару")
                                        .Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Кількість").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ціна").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Сума").Bold();
                                });

                                int index = 1;
                                foreach (var item in order.OrderItems)
                                {
                                    var bgColor = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                    table.Cell().Background(bgColor).Padding(5).Text(index.ToString());
                                    table.Cell().Background(bgColor).Padding(5).Text(item.ProductName);
                                    table.Cell().Background(bgColor).Padding(5).Text(item.Quantity.ToString());
                                    table.Cell().Background(bgColor).Padding(5).Text($"{item.Price:N2} грн");
                                    table.Cell().Background(bgColor).Padding(5).Text($"{item.TotalPrice:N2} грн");

                                    index++;
                                }
                            });

                            column.Item().PaddingTop(20);

                            column.Item().AlignRight().Column(col =>
                            {
                                var subtotal = order.TotalAmount - order.DeliveryPrice;

                                col.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium)
                                    .PaddingBottom(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("Вартість товарів:");
                                        row.ConstantItem(100).AlignRight().Text($"{subtotal:N2} грн");
                                    });

                                col.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Доставка:");
                                    row.ConstantItem(100).AlignRight().Text($"{order.DeliveryPrice:N2} грн");
                                });

                                col.Item().PaddingTop(10).BorderTop(2).BorderColor(Colors.Black)
                                    .PaddingTop(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("РАЗОМ ДО СПЛАТИ:").Bold().FontSize(14);
                                        row.ConstantItem(100).AlignRight().Text($"{order.TotalAmount:N2} грн")
                                            .Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                                    });
                            });

                            column.Item().PaddingTop(20);
                            column.Item().Background(Colors.Grey.Lighten4).Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text($"Спосіб оплати: {order.PaymentMethod}");
                                    col.Item().Text($"Спосіб доставки: {order.DeliveryMethod}");
                                    if (!string.IsNullOrEmpty(order.Notes))
                                    {
                                        col.Item().PaddingTop(5).Text($"Коментар: {order.Notes}");
                                    }
                                });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Дякуємо за покупку! | ");
                            text.Span("www.techshop.ua").FontColor(Colors.Blue.Medium);
                        });
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