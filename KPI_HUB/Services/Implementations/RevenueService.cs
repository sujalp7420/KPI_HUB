using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class RevenueService : IRevenueService
{
    private readonly IRevenueRepository _revenueRepository;
    private readonly ICustomerRepository _customerRepository;

    public RevenueService(IRevenueRepository revenueRepository, ICustomerRepository customerRepository)
    {
        _revenueRepository = revenueRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<RevenueListViewModel>> GetRevenueAsync()
    {
        var revenues = await _revenueRepository.GetAllAsync();
        var customers = (await _customerRepository.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        return revenues.OrderByDescending(r => r.Date).Select(r => new RevenueListViewModel
        {
            Id = r.Id,
            Date = r.Date,
            Amount = r.Amount,
            Source = r.Source,
            Description = r.Description,
            CustomerName = r.CustomerId.HasValue && customers.ContainsKey(r.CustomerId.Value) ? customers[r.CustomerId.Value] : "N/A",
            PaymentMethod = r.PaymentMethod
        });
    }

    public async Task<EditRevenueViewModel?> GetRevenueForEditAsync(int id)
    {
        var revenue = await _revenueRepository.GetByIdAsync(id);
        if (revenue == null) return null;

        return new EditRevenueViewModel
        {
            Id = revenue.Id,
            Date = revenue.Date,
            Amount = revenue.Amount,
            Source = revenue.Source,
            Description = revenue.Description,
            CustomerId = revenue.CustomerId,
            PaymentMethod = revenue.PaymentMethod,
            ReferenceNumber = revenue.ReferenceNumber
        };
    }

    public async Task AddRevenueAsync(CreateRevenueViewModel model)
    {
        var revenue = new Revenue
        {
            Date = model.Date,
            Amount = model.Amount,
            Source = model.Source,
            Description = model.Description,
            CustomerId = model.CustomerId,
            PaymentMethod = model.PaymentMethod,
            ReferenceNumber = model.ReferenceNumber
        };

        if (model.CustomerId.HasValue)
        {
            var customer = await _customerRepository.GetByIdAsync(model.CustomerId.Value);
            if (customer != null)
            {
                customer.TotalPurchases += model.Amount;
                _customerRepository.Update(customer);
            }
        }

        await _revenueRepository.AddAsync(revenue);
        await _revenueRepository.SaveAsync();
    }

    public async Task UpdateRevenueAsync(EditRevenueViewModel model)
    {
        var revenue = await _revenueRepository.GetByIdAsync(model.Id);
        if (revenue == null) return;

        revenue.Date = model.Date;
        revenue.Amount = model.Amount;
        revenue.Source = model.Source;
        revenue.Description = model.Description;
        revenue.CustomerId = model.CustomerId;
        revenue.PaymentMethod = model.PaymentMethod;
        revenue.ReferenceNumber = model.ReferenceNumber;

        _revenueRepository.Update(revenue);
        await _revenueRepository.SaveAsync();
    }

    public async Task DeleteRevenueAsync(int id)
    {
        var revenue = await _revenueRepository.GetByIdAsync(id);
        if (revenue == null) return;

        _revenueRepository.Delete(revenue);
        await _revenueRepository.SaveAsync();
    }
}
