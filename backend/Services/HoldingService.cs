using Backend.Models;
using Backend.Repositories;

namespace Backend.Services;

public class HoldingService : IHoldingService
{
    private readonly IHoldingRepository _holdingRepo;
    private readonly IPriceRepository _priceRepo;

    public HoldingService(IHoldingRepository holdingRepo, IPriceRepository priceRepo)
    {
        _holdingRepo = holdingRepo;
        _priceRepo = priceRepo;
    }

    public async Task<List<HoldingDto>> GetAllHoldingsAsync()
    {
        var holdings = await _holdingRepo.GetAllAsync();
        var prices = await _priceRepo.GetAllAsync();

        // turn price list into a dictionary for fast lookup
        // { "AAPL": 189.50, "MSFT": 415.20, ... }
        var priceMap = prices.ToDictionary(p => p.Ticker.ToUpper(), p => p.CurrentPrice);

        return holdings.Select(h =>
        {
            var currentPrice = priceMap.TryGetValue(h.Ticker.ToUpper(), out var p) ? p : h.PurchasePrice;
            var marketValue = currentPrice * h.Quantity;
            var unrealizedPnL = (currentPrice - h.PurchasePrice) * h.Quantity;

            return new HoldingDto
            {
                Id = h.Id,
                Ticker = h.Ticker,
                Quantity = h.Quantity,
                PurchasePrice = h.PurchasePrice,
                CurrentPrice = currentPrice,
                MarketValue = marketValue,
                UnrealizedPnL = unrealizedPnL,
                CreatedAt = h.CreatedAt
            };
        }).ToList();
    }

    public async Task<HoldingDto> AddHoldingAsync(CreateHoldingDto dto)
    {
        // validation
        if (string.IsNullOrWhiteSpace(dto.Ticker))
            throw new ArgumentException("Ticker is required");

        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (dto.PurchasePrice <= 0)
            throw new ArgumentException("Purchase price must be greater than zero");

        // check ticker exists in prices table
        var price = await _priceRepo.GetByTickerAsync(dto.Ticker);
        if (price == null)
            throw new ArgumentException($"Ticker {dto.Ticker.ToUpper()} not found. Valid tickers: AAPL, MSFT, JPM, T, GS");

        // check for duplicate ticker
        var existing = await _holdingRepo.GetByTickerAsync(dto.Ticker);
        if (existing != null)
            throw new InvalidOperationException($"Holding for {dto.Ticker.ToUpper()} already exists");

        // new holding
        var holding = new Holding
        {
            Ticker = dto.Ticker.ToUpper(),
            Quantity = dto.Quantity,
            PurchasePrice = dto.PurchasePrice,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _holdingRepo.AddAsync(holding);

        return new HoldingDto
        {
            Id = created.Id,
            Ticker = created.Ticker,
            Quantity = created.Quantity,
            PurchasePrice = created.PurchasePrice,
            CurrentPrice = price.CurrentPrice,
            MarketValue = price.CurrentPrice * created.Quantity,
            UnrealizedPnL = (price.CurrentPrice - created.PurchasePrice) * created.Quantity,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task DeleteHoldingAsync(int id)
    {
        var holding = await _holdingRepo.GetByIdAsync(id);
        if (holding == null)
            throw new KeyNotFoundException($"Holding with id {id} not found");

        await _holdingRepo.DeleteAsync(holding);
    }
}