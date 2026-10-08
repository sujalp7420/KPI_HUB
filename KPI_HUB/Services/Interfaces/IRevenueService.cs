using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IRevenueService
{
    Task<IEnumerable<RevenueListViewModel>> GetRevenueAsync();
    Task<EditRevenueViewModel?> GetRevenueForEditAsync(int id);
    Task AddRevenueAsync(CreateRevenueViewModel model);
    Task UpdateRevenueAsync(EditRevenueViewModel model);
    Task DeleteRevenueAsync(int id);
}
