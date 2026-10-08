using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<SaleListViewModel>> GetSalesAsync()
    {
        var sales = await _saleRepository.GetAllWithDetailsAsync();
        return sales.Select(s => new SaleListViewModel
        {
            Id = s.Id,
            CustomerName = s.Customer?.Name ?? "Unknown",
            SaleDate = s.SaleDate,
            TotalAmount = s.TotalAmount,
            PaymentMethod = s.PaymentMethod,
            Status = s.Status
        });
    }

    public async Task<SaleDetailViewModel?> GetSaleDetailAsync(int id)
    {
        var sale = await _saleRepository.GetByIdWithItemsAsync(id);
        if (sale == null) return null;

        return new SaleDetailViewModel
        {
            Id = sale.Id,
            CustomerName = sale.Customer?.Name ?? "Unknown",
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod,
            Status = sale.Status,
            Items = sale.SaleItems.Select(si => new SaleItemViewModel
            {
                ProductId = si.ProductId,
                ProductName = si.Product?.Name ?? "Unknown",
                Quantity = (int)si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.TotalPrice
            }).ToList()
        };
    }

    public async Task AddSaleAsync(CreateSaleViewModel model)
    {
        var sale = new Sale
        {
            CustomerId = model.CustomerId,
            SaleDate = model.SaleDate,
            PaymentMethod = model.PaymentMethod,
            Status = model.Status,
            SaleItems = new List<SaleItem>()
        };

        decimal totalAmount = 0;

        foreach (var item in model.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            var unitPrice = item.UnitPrice > 0 ? item.UnitPrice : (product?.Price ?? 0);
            var itemTotal = unitPrice * item.Quantity;

            sale.SaleItems.Add(new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = itemTotal
            });

            totalAmount += itemTotal;

            if (product != null)
            {
                product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                _productRepository.Update(product);
            }
        }

        sale.TotalAmount = totalAmount;

        var customer = await _customerRepository.GetByIdAsync(model.CustomerId);
        if (customer != null)
        {
            customer.TotalPurchases += totalAmount;
            _customerRepository.Update(customer);
        }

        await _saleRepository.AddAsync(sale);
        await _saleRepository.SaveAsync();
    }

    public async Task DeleteSaleAsync(int id)
    {
        var sale = await _saleRepository.GetByIdWithItemsAsync(id);
        if (sale == null) return;

        // Restore product stock
        foreach (var item in sale.SaleItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += (int)item.Quantity;
                _productRepository.Update(product);
            }
        }

        var customer = await _customerRepository.GetByIdAsync(sale.CustomerId);
        if (customer != null)
        {
            customer.TotalPurchases = Math.Max(0, customer.TotalPurchases - sale.TotalAmount);
            _customerRepository.Update(customer);
        }

        _saleRepository.Delete(sale);
        await _saleRepository.SaveAsync();
    }
}
