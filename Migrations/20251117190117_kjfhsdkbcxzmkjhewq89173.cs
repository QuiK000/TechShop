using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class kjfhsdkbcxzmkjhewq89173 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5443));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5446));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5447));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5448));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5449));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5450));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5593), new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5593) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5600), new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5600) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5602), new DateTime(2025, 11, 17, 19, 1, 16, 298, DateTimeKind.Utc).AddTicks(5602) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8819));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8822));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8823));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8824));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8825));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8826));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8990), new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(8990) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(9030), new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(9030) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(9033), new DateTime(2025, 11, 17, 15, 22, 10, 439, DateTimeKind.Utc).AddTicks(9033) });
        }
    }
}
