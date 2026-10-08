using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetLowStockProductsAsync();
    Task<IEnumerable<Product>> GetTopSellingProductsAsync();
    Task<Product?> GetByIdWithSalesAsync(int id);
}
