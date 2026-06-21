using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Holding> Holdings { get; set; }
    public DbSet<Price> Prices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Price>().HasData(
            new Price { Id = 1,  Ticker = "AAPL", CurrentPrice = 189.50m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 2,  Ticker = "MSFT", CurrentPrice = 415.20m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 3,  Ticker = "JPM",  CurrentPrice = 198.75m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 4,  Ticker = "T",    CurrentPrice = 18.90m,  LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 5,  Ticker = "GS",   CurrentPrice = 456.30m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 6,  Ticker = "AMZN", CurrentPrice = 178.25m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 7,  Ticker = "NVDA", CurrentPrice = 875.40m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 8,  Ticker = "META", CurrentPrice = 512.60m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 9,  Ticker = "BRK",  CurrentPrice = 350.00m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Price { Id = 10, Ticker = "V",    CurrentPrice = 267.80m, LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}