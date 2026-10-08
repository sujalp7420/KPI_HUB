using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Sales
            .Where(s => s.SaleDate >= start && s.SaleDate <= end)
            .Include(s => s.Customer)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Sales.SumAsync(s => s.TotalAmount);
    }
}
