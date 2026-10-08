using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Expenses
            .Where(e => e.Date >= start && e.Date <= end)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalExpensesAsync()
    {
        return await _context.Expenses.SumAsync(e => e.Amount);
    }
}
