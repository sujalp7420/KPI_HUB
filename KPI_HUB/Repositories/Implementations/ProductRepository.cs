using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
    {
        return await _context.Products
            .Where(p => p.StockQuantity <= p.MinimumStock)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetTopSellingProductsAsync()
    {
        return await _context.Products
            .Include(p => p.SaleItems)
            .OrderByDescending(p => p.SaleItems.Sum(si => si.Quantity))
            .Take(5)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithSalesAsync(int id)
    {
        return await _context.Products
            .Include(p => p.SaleItems)
            .ThenInclude(si => si.Sale)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
