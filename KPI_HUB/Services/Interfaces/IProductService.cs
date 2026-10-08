using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductListViewModel>> GetProductsAsync();
    Task<ProductFormViewModel?> GetProductForEditAsync(int id);
    Task AddProductAsync(ProductFormViewModel model);
    Task UpdateProductAsync(ProductFormViewModel model);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<ProductListViewModel>> GetLowStockProductsAsync();
}
