using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface ISaleService
{
    Task<IEnumerable<SaleListViewModel>> GetSalesAsync();
    Task<SaleDetailViewModel?> GetSaleDetailAsync(int id);
    Task AddSaleAsync(CreateSaleViewModel model);
    Task DeleteSaleAsync(int id);
}
