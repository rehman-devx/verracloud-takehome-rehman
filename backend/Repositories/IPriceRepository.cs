using Backend.Models;

namespace Backend.Repositories;

public interface IPriceRepository
{
    Task<List<Price>> GetAllAsync();
    Task<Price?> GetByTickerAsync(string ticker);
    Task UpdatePriceAsync(Price price);
}