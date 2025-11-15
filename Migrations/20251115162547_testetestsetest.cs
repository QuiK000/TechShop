using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class testetestsetest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1472));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1476));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1477));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1479));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1675), new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1675) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1682), new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1683) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1685), new DateTime(2025, 11, 15, 16, 25, 39, 122, DateTimeKind.Utc).AddTicks(1685) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4476));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4480));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4481));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4482));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4483));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4484));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4657), new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4657) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4663), new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4663) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4665), new DateTime(2025, 11, 15, 15, 4, 1, 468, DateTimeKind.Utc).AddTicks(4666) });
        }
    }
}
