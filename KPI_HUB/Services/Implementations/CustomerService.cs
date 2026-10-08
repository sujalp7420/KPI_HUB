using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerListViewModel>> GetCustomersAsync()
    {
        var customers = await _customerRepository.GetAllWithSalesAsync();
        return customers.Select(c => new CustomerListViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Status = c.Status,
            TotalPurchases = c.Sales.Sum(s => s.TotalAmount) + c.TotalPurchases,
            RegistrationDate = c.RegistrationDate
        });
    }

    public async Task<CustomerDetailViewModel?> GetCustomerDetailsAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithSalesAsync(id);
        if (customer == null) return null;

        var sales = customer.Sales.OrderByDescending(s => s.SaleDate).ToList();

        return new CustomerDetailViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            RegistrationDate = customer.RegistrationDate,
            Status = customer.Status,
            TotalPurchases = sales.Sum(s => s.TotalAmount) + customer.TotalPurchases,
            PurchaseHistory = sales.Select(s => new CustomerPurchaseHistoryItem
            {
                SaleId = s.Id,
                SaleDate = s.SaleDate,
                TotalAmount = s.TotalAmount
            }).ToList()
        };
    }

    public async Task AddCustomerAsync(CreateCustomerViewModel model)
    {
        var customer = new Customer
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            Status = model.Status,
            RegistrationDate = DateTime.UtcNow,
            TotalPurchases = 0
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveAsync();
    }

    public async Task UpdateCustomerAsync(EditCustomerViewModel model)
    {
        var customer = await _customerRepository.GetByIdAsync(model.Id);
        if (customer == null) return;

        customer.Name = model.Name;
        customer.Email = model.Email;
        customer.Phone = model.Phone;
        customer.Address = model.Address;
        customer.Status = model.Status;

        _customerRepository.Update(customer);
        await _customerRepository.SaveAsync();
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return;

        _customerRepository.Delete(customer);
        await _customerRepository.SaveAsync();
    }
}
