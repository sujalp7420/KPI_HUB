using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalExpensesAsync();
}
