using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface IRevenueRepository : IRepository<Revenue>
{
    Task<IEnumerable<Revenue>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalRevenueAsync();
}
