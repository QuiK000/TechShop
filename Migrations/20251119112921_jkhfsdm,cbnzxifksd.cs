using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class jkhfsdmcbnzxifksd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6973));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6976));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6977));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6978));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7118), new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7118) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7126), new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7126) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7128), new DateTime(2025, 11, 19, 11, 29, 5, 529, DateTimeKind.Utc).AddTicks(7128) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
