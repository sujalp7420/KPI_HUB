using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetAllWithSalesAsync()
    {
        return await _context.Customers
            .Include(c => c.Sales)
            .ThenInclude(s => s.SaleItems)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdWithSalesAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Sales)
            .ThenInclude(s => s.SaleItems)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return await _context.Customers.CountAsync();
    }

    public async Task<int> GetNewCustomersThisMonthAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Customers
            .CountAsync(c => c.RegistrationDate >= new DateTime(now.Year, now.Month, 1) && c.RegistrationDate <= now);
    }
}
