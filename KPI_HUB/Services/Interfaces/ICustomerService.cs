using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerListViewModel>> GetCustomersAsync();
    Task<CustomerDetailViewModel?> GetCustomerDetailsAsync(int id);
    Task AddCustomerAsync(CreateCustomerViewModel model);
    Task UpdateCustomerAsync(EditCustomerViewModel model);
    Task DeleteCustomerAsync(int id);
}
