using Backend.Repositories;

namespace Backend.Services;

public class PriceUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceUpdateService> _logger;

    public PriceUpdateService(IServiceScopeFactory scopeFactory, ILogger<PriceUpdateService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price update service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            try
            {
                await RefreshPricesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing prices");
            }
        }
    }

    private async Task RefreshPricesAsync()
    {
        // background services are singletons
        // DbContext is scoped
        // must create a scope to resolve scoped services
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

        _logger.LogInformation("Prices refreshed at {time}", DateTime.UtcNow);
    }
}