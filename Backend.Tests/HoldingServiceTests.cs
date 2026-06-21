using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Moq;

namespace Backend.Tests;

public class HoldingServiceTests
{
    private readonly Mock<IHoldingRepository> _holdingRepo;
    private readonly Mock<IPriceRepository> _priceRepo;
    private readonly HoldingService _service;

    public HoldingServiceTests()
    {
        _holdingRepo = new Mock<IHoldingRepository>();
        _priceRepo = new Mock<IPriceRepository>();
        _service = new HoldingService(_holdingRepo.Object, _priceRepo.Object);
    }

    // ── P&L Calculation Tests ──────────────────────────────

    [Fact]
    public async Task GetAllHoldings_CalculatesProfit_WhenCurrentPriceAbovePurchasePrice()
    {
        // Arrange
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "AAPL", Quantity = 100, PurchasePrice = 150.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>
        {
            new Price { Ticker = "AAPL", CurrentPrice = 200.00m }
        });

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(200.00m, result[0].CurrentPrice);
        Assert.Equal(20000.00m, result[0].MarketValue);      // 200 × 100
        Assert.Equal(5000.00m, result[0].UnrealizedPnL);     // (200 - 150) × 100
    }

    [Fact]
    public async Task GetAllHoldings_CalculatesLoss_WhenCurrentPriceBelowPurchasePrice()
    {
        // Arrange
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "AAPL", Quantity = 100, PurchasePrice = 150.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>
        {
            new Price { Ticker = "AAPL", CurrentPrice = 120.00m }
        });

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert
        Assert.Equal(120.00m, result[0].CurrentPrice);
        Assert.Equal(12000.00m, result[0].MarketValue);      // 120 × 100
        Assert.Equal(-3000.00m, result[0].UnrealizedPnL);    // (120 - 150) × 100
    }

    [Fact]
    public async Task GetAllHoldings_ReturnsZeroPnL_WhenCurrentPriceEqualsPurchasePrice()
    {
        // Arrange
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "AAPL", Quantity = 100, PurchasePrice = 150.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>
        {
            new Price { Ticker = "AAPL", CurrentPrice = 150.00m }
        });

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert
        Assert.Equal(0.00m, result[0].UnrealizedPnL);
        Assert.Equal(15000.00m, result[0].MarketValue);
    }

    [Fact]
    public async Task GetAllHoldings_FallsBackToPurchasePrice_WhenNoCurrentPriceExists()
    {
        // Arrange — holding exists but no price in Prices table
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "AAPL", Quantity = 100, PurchasePrice = 150.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>()); // empty

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert — falls back to purchase price, zero P&L
        Assert.Equal(150.00m, result[0].CurrentPrice);
        Assert.Equal(0.00m, result[0].UnrealizedPnL);
    }

    [Fact]
    public async Task GetAllHoldings_CalculatesCorrectly_WithLargeQuantity()
    {
        // Arrange
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "T", Quantity = 10000, PurchasePrice = 16.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>
        {
            new Price { Ticker = "T", CurrentPrice = 18.90m }
        });

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert
        Assert.Equal(189000.00m, result[0].MarketValue);     // 18.90 × 10000
        Assert.Equal(29000.00m, result[0].UnrealizedPnL);    // (18.90 - 16) × 10000
    }

    [Fact]
    public async Task GetAllHoldings_CalculatesMultipleHoldings_Correctly()
    {
        // Arrange
        _holdingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Holding>
        {
            new Holding { Id = 1, Ticker = "AAPL", Quantity = 100, PurchasePrice = 150.00m },
            new Holding { Id = 2, Ticker = "MSFT", Quantity = 50,  PurchasePrice = 380.00m }
        });
        _priceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Price>
        {
            new Price { Ticker = "AAPL", CurrentPrice = 200.00m },
            new Price { Ticker = "MSFT", CurrentPrice = 350.00m }
        });

        // Act
        var result = await _service.GetAllHoldingsAsync();

        // Assert AAPL
        Assert.Equal(5000.00m, result[0].UnrealizedPnL);     // profit

        // Assert MSFT
        Assert.Equal(-1500.00m, result[1].UnrealizedPnL);    // loss
    }

    // ── Validation Tests ───────────────────────────────────

    [Fact]
    public async Task AddHolding_ThrowsArgumentException_WhenTickerIsEmpty()
    {
        var dto = new CreateHoldingDto { Ticker = "", Quantity = 10, PurchasePrice = 100 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task AddHolding_ThrowsArgumentException_WhenQuantityIsZero()
    {
        var dto = new CreateHoldingDto { Ticker = "AAPL", Quantity = 0, PurchasePrice = 100 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task AddHolding_ThrowsArgumentException_WhenQuantityIsNegative()
    {
        var dto = new CreateHoldingDto { Ticker = "AAPL", Quantity = -10, PurchasePrice = 100 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task AddHolding_ThrowsArgumentException_WhenPurchasePriceIsZero()
    {
        var dto = new CreateHoldingDto { Ticker = "AAPL", Quantity = 10, PurchasePrice = 0 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task AddHolding_ThrowsArgumentException_WhenTickerNotInPricesTable()
    {
        _priceRepo.Setup(r => r.GetByTickerAsync("FAKE")).ReturnsAsync((Price?)null);

        var dto = new CreateHoldingDto { Ticker = "FAKE", Quantity = 10, PurchasePrice = 100 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task AddHolding_ThrowsInvalidOperationException_WhenTickerAlreadyExists()
    {
        _priceRepo.Setup(r => r.GetByTickerAsync("AAPL"))
            .ReturnsAsync(new Price { Ticker = "AAPL", CurrentPrice = 189.50m });

        _holdingRepo.Setup(r => r.GetByTickerAsync("AAPL"))
            .ReturnsAsync(new Holding { Id = 1, Ticker = "AAPL" });

        var dto = new CreateHoldingDto { Ticker = "AAPL", Quantity = 10, PurchasePrice = 150 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddHoldingAsync(dto));
    }

    [Fact]
    public async Task DeleteHolding_ThrowsKeyNotFoundException_WhenHoldingDoesNotExist()
    {
        _holdingRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Holding?)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteHoldingAsync(999));
    }
}