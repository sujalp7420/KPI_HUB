using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class RevenueRepository : Repository<Revenue>, IRevenueRepository
{
    public RevenueRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Revenue>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Revenues
            .Where(r => r.Date >= start && r.Date <= end)
            .Include(r => r.Customer)
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Revenues.SumAsync(r => r.Amount);
    }
}
