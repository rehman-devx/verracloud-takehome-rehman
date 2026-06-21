using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly AppDbContext _context;

    public PriceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Price>> GetAllAsync()
    {
        return await _context.Prices
            .AsNoTracking()
            .OrderBy(p => p.Ticker)
            .ToListAsync();
    }

    public async Task<Price?> GetByTickerAsync(string ticker)
    {
        return await _context.Prices
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Ticker.ToUpper() == ticker.ToUpper());
    }

    public async Task UpdatePriceAsync(Price price)
    {
        _context.Prices.Update(price);
        await _context.SaveChangesAsync();
    }
}