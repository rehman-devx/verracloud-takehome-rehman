using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holdings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticker = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holdings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticker = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "Id", "CurrentPrice", "LastUpdatedAt", "Ticker" },
                values: new object[,]
                {
                    { 1, 189.50m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AAPL" },
                    { 2, 415.20m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MSFT" },
                    { 3, 198.75m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "JPM" },
                    { 4, 18.90m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "T" },
                    { 5, 456.30m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GS" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holdings");

            migrationBuilder.DropTable(
                name: "Prices");
        }
    }
}
