using Backend.Hubs;
using Backend.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class PriceUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceUpdateService> _logger;
    private readonly IHubContext<PriceHub> _hubContext;

    public PriceUpdateService(
        IServiceScopeFactory scopeFactory,
        ILogger<PriceUpdateService> logger,
        IHubContext<PriceHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price update service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            try
            {
                var prices = await RefreshPricesAsync();

                // broadcast to ALL connected frontend clients
                await _hubContext.Clients.All.SendAsync("PricesUpdated", prices, stoppingToken);

                _logger.LogInformation("Prices refreshed and broadcast at {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing prices");
            }
        }
    }

    private async Task<List<object>> RefreshPricesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository>();

        var prices = await priceRepo.GetAllAsync();
        var random = new Random();

        foreach (var price in prices)
        {
            var change = 1 + (random.NextDouble() * 0.04 - 0.02);
            price.CurrentPrice = Math.Round((decimal)(price.CurrentPrice * (decimal)change), 2);
            price.LastUpdatedAt = DateTime.UtcNow;
            await priceRepo.UpdatePriceAsync(price);
        }

        // return lowercase for JavaScript
        return prices.Select(p => (object)new {
            ticker = p.Ticker.ToUpper(),
            currentPrice = p.CurrentPrice,
            lastUpdatedAt = p.LastUpdatedAt
        }).ToList();
    }
}