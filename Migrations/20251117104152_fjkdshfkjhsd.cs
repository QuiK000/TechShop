using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class fjkdshfkjhsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1081));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1084));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1085));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1086));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1087));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1088));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1336), new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1337) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1352), new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1352) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1354), new DateTime(2025, 11, 17, 10, 41, 50, 997, DateTimeKind.Utc).AddTicks(1355) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5321));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5357));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5358));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5359));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5360));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5361));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5501), new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5501) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5510), new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5511) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5513), new DateTime(2025, 11, 16, 17, 37, 20, 881, DateTimeKind.Utc).AddTicks(5513) });
        }
    }
}
