using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreTickers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "Id", "CurrentPrice", "LastUpdatedAt", "Ticker" },
                values: new object[,]
                {
                    { 6, 178.25m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AMZN" },
                    { 7, 875.40m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NVDA" },
                    { 8, 512.60m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "META" },
                    { 9, 350.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRK" },
                    { 10, 267.80m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "V" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
