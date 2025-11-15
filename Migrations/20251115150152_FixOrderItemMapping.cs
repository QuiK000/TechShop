using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2761));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2773));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2774));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2775));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2776));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(2777));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3172), new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3172) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3182), "/images/products/laptop1.jpg", new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3182) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3184), "/images/products/laptop1.jpg", new DateTime(2025, 11, 15, 15, 1, 49, 150, DateTimeKind.Utc).AddTicks(3184) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(211));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(259));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(260));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(261));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(262));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(263));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(605), new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(605) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(613), "/images/products/laptop2.jpg", new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(613) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(615), "/images/products/mouse1.jpg", new DateTime(2025, 11, 15, 13, 11, 41, 8, DateTimeKind.Utc).AddTicks(615) });
        }
    }
}
