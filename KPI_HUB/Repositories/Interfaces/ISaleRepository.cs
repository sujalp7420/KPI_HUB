using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface ISaleRepository : IRepository<Sale>
{
    Task<IEnumerable<Sale>> GetAllWithDetailsAsync();
    Task<Sale?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalRevenueAsync();
}
