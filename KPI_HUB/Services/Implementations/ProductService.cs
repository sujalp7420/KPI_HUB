using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductListViewModel>> GetProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => new ProductListViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Price = p.Price,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            MinimumStock = p.MinimumStock,
            Status = p.Status
        });
    }

    public async Task<ProductFormViewModel?> GetProductForEditAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            MinimumStock = product.MinimumStock,
            Status = product.Status
        };
    }

    public async Task AddProductAsync(ProductFormViewModel model)
    {
        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Category = model.Category,
            Price = model.Price,
            CostPrice = model.CostPrice,
            StockQuantity = model.StockQuantity,
            MinimumStock = model.MinimumStock,
            Status = model.Status
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveAsync();
    }

    public async Task UpdateProductAsync(ProductFormViewModel model)
    {
        var product = await _productRepository.GetByIdAsync(model.Id);
        if (product == null) return;

        product.Name = model.Name;
        product.Description = model.Description;
        product.Category = model.Category;
        product.Price = model.Price;
        product.CostPrice = model.CostPrice;
        product.StockQuantity = model.StockQuantity;
        product.MinimumStock = model.MinimumStock;
        product.Status = model.Status;

        _productRepository.Update(product);
        await _productRepository.SaveAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return;

        _productRepository.Delete(product);
        await _productRepository.SaveAsync();
    }

    public async Task<IEnumerable<ProductListViewModel>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetLowStockProductsAsync();
        return products.Select(p => new ProductListViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Price = p.Price,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            MinimumStock = p.MinimumStock,
            Status = p.Status
        });
    }
}
