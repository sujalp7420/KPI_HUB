using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetAllWithSalesAsync();
    Task<Customer?> GetByIdWithSalesAsync(int id);
    Task<int> GetTotalCustomersAsync();
    Task<int> GetNewCustomersThisMonthAsync();
}
