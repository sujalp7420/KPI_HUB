using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IRevenueRepository _revenueRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMarketingCampaignRepository _marketingRepository;

    public DashboardService(
        IRevenueRepository revenueRepository,
        IExpenseRepository expenseRepository,
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IMarketingCampaignRepository marketingRepository)
    {
        _revenueRepository = revenueRepository;
        _expenseRepository = expenseRepository;
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _marketingRepository = marketingRepository;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var revenues = (await _revenueRepository.GetAllAsync()).ToList();
        var sales = (await _saleRepository.GetAllWithDetailsAsync()).ToList();
        var expenses = (await _expenseRepository.GetAllAsync()).ToList();
        var customers = (await _customerRepository.GetAllAsync()).ToList();
        var products = (await _productRepository.GetAllAsync()).ToList();
        var campaigns = (await _marketingRepository.GetAllAsync()).ToList();
        var lowStockProducts = (await _productRepository.GetLowStockProductsAsync()).ToList();

        var directRevenue = revenues.Sum(r => r.Amount);
        var salesRevenue = sales.Sum(s => s.TotalAmount);
        var totalRevenue = directRevenue + salesRevenue;
        var totalExpenses = expenses.Sum(e => e.Amount);
        var netProfit = totalRevenue - totalExpenses;
        var profitMargin = totalRevenue > 0 ? Math.Round((netProfit / totalRevenue) * 100m, 2) : 0m;
        var totalSalesCount = sales.Count;
        var aov = totalSalesCount > 0 ? Math.Round(salesRevenue / totalSalesCount, 2) : 0m;

        var totalMarketingCost = campaigns.Sum(c => c.ActualCost);
        var totalMarketingRev = campaigns.Sum(c => c.RevenueGenerated);
        var marketingRoi = totalMarketingCost > 0 ? Math.Round(((totalMarketingRev - totalMarketingCost) / totalMarketingCost) * 100m, 2) : 0m;

        // Last 6 months trend
        var now = DateTime.UtcNow;
        var sixMonthsAgo = now.AddMonths(-5);
        var startOfMonth = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);

        var monthlyRevenueExpense = new List<MonthlyTrendItem>();
        var monthlyProfit = new List<MonthlyProfitItem>();
        var customerGrowth = new List<CustomerGrowthItem>();

        for (int i = 0; i < 6; i++)
        {
            var targetMonth = startOfMonth.AddMonths(i);
            var monthLabel = targetMonth.ToString("MMM yyyy");
            var monthRevenues = revenues.Where(r => r.Date.Year == targetMonth.Year && r.Date.Month == targetMonth.Month).Sum(r => r.Amount)
                              + sales.Where(s => s.SaleDate.Year == targetMonth.Year && s.SaleDate.Month == targetMonth.Month).Sum(s => s.TotalAmount);
            var monthExpenses = expenses.Where(e => e.Date.Year == targetMonth.Year && e.Date.Month == targetMonth.Month).Sum(e => e.Amount);
            var mProfit = monthRevenues - monthExpenses;
            var newCusts = customers.Count(c => c.RegistrationDate.Year == targetMonth.Year && c.RegistrationDate.Month == targetMonth.Month);

            monthlyRevenueExpense.Add(new MonthlyTrendItem
            {
                Label = monthLabel,
                Revenue = monthRevenues,
                Expenses = monthExpenses
            });

            monthlyProfit.Add(new MonthlyProfitItem
            {
                Label = monthLabel,
                Profit = mProfit
            });

            customerGrowth.Add(new CustomerGrowthItem
            {
                Label = monthLabel,
                Count = newCusts
            });
        }

        // Sales by product
        var salesByProduct = sales.SelectMany(s => s.SaleItems)
            .GroupBy(si => si.Product?.Name ?? $"Product #{si.ProductId}")
            .Select(g => new ProductSalesItem
            {
                Label = g.Key,
                Quantity = (int)g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(6)
            .ToList();

        // Expenses by category
        var expenseByCategory = expenses
            .GroupBy(e => string.IsNullOrWhiteSpace(e.Category) ? "Other" : e.Category)
            .Select(g => new ExpenseCategoryItem
            {
                Label = g.Key,
                Value = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        // Marketing performance
        var marketingPerformance = campaigns
            .Select(c => new MarketingPerformanceItem
            {
                Label = c.Name,
                Revenue = c.RevenueGenerated,
                Cost = c.ActualCost
            })
            .Take(6)
            .ToList();

        // Smart insights / Alerts
        var insights = new List<InsightItem>();

        if (netProfit > 0)
        {
            insights.Add(new InsightItem
            {
                Message = $"Healthy financial health: Net profit is ₹{netProfit:N2} with a {profitMargin}% margin.",
                Tone = "success"
            });
        }
        else if (netProfit < 0)
        {
            insights.Add(new InsightItem
            {
                Message = $"Attention: Net loss is currently ₹{Math.Abs(netProfit):N2}. Review operating expenses.",
                Tone = "danger"
            });
        }

        if (lowStockProducts.Count > 0)
        {
            insights.Add(new InsightItem
            {
                Message = $"Inventory alert: {lowStockProducts.Count} product(s) are at or below minimum reorder level.",
                Tone = "warning"
            });
        }

        if (marketingRoi > 0)
        {
            insights.Add(new InsightItem
            {
                Message = $"Marketing efficiency: Total campaigns generated {marketingRoi}% ROI across all active platforms.",
                Tone = "info"
            });
        }

        var lowStockViewModels = lowStockProducts.Select(p => new ProductListViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Price = p.Price,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            MinimumStock = p.MinimumStock,
            Status = p.Status
        }).ToList();

        return new DashboardViewModel
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            ProfitMargin = profitMargin,
            TotalCustomers = customers.Count,
            TotalProducts = products.Count,
            TotalSales = totalSalesCount,
            AverageOrderValue = aov,
            MarketingRoi = marketingRoi,
            MonthlyRevenueExpense = monthlyRevenueExpense,
            MonthlyProfit = monthlyProfit,
            CustomerGrowth = customerGrowth,
            SalesByProduct = salesByProduct,
            ExpenseByCategory = expenseByCategory,
            MarketingPerformance = marketingPerformance,
            Insights = insights,
            LowStockProducts = lowStockViewModels
        };
    }
}
