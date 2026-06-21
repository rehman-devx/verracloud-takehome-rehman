using Microsoft.AspNetCore.Mvc;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPriceRepository _priceRepo;

    public PricesController(IPriceRepository priceRepo)
    {
        _priceRepo = priceRepo;
    }

    // GET api/prices
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prices = await _priceRepo.GetAllAsync();
        return Ok(prices);
    }

    // POST api/prices/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var prices = await _priceRepo.GetAllAsync();
        var random = new Random();

        foreach (var price in prices)
        {
            // randomize price ±2%
            var change = 1 + (random.NextDouble() * 0.04 - 0.02);
            price.CurrentPrice = Math.Round((decimal)(price.CurrentPrice * (decimal)change), 2);
            price.LastUpdatedAt = DateTime.UtcNow;
            await _priceRepo.UpdatePriceAsync(price);
        }

        var updated = await _priceRepo.GetAllAsync();
        return Ok(updated);
    }
}