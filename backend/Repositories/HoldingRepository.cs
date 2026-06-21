using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Repositories;

public class HoldingRepository : IHoldingRepository
{
    private readonly AppDbContext _context;

    public HoldingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Holding>> GetAllAsync()
    {
        return await _context.Holdings
            .AsNoTracking()
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<Holding?> GetByIdAsync(int id)
    {
        return await _context.Holdings
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Holding?> GetByTickerAsync(string ticker)
    {
        return await _context.Holdings
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Ticker.ToUpper() == ticker.ToUpper());
    }

    public async Task<Holding> AddAsync(Holding holding)
    {
        _context.Holdings.Add(holding);
        await _context.SaveChangesAsync();
        return holding;
    }

    public async Task DeleteAsync(Holding holding)
    {
        _context.Holdings.Remove(holding);
        await _context.SaveChangesAsync();
    }
}