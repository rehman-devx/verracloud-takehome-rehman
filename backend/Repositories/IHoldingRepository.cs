using Backend.Models;

namespace Backend.Repositories;

public interface IHoldingRepository
{
    Task<List<Holding>> GetAllAsync();
    Task<Holding?> GetByIdAsync(int id);
    Task<Holding?> GetByTickerAsync(string ticker);
    Task<Holding> AddAsync(Holding holding);
    Task DeleteAsync(Holding holding);
}