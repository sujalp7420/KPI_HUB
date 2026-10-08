using ClosedXML.Excel;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IRevenueRepository _revenueRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMarketingCampaignRepository _marketingRepository;

    public ReportService(
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

    public async Task<ReportOverviewViewModel> GetOverviewAsync()
    {
        var revenues = await _revenueRepository.GetAllAsync();
        var sales = await _saleRepository.GetAllAsync();
        var expenses = await _expenseRepository.GetAllAsync();
        var customers = await _customerRepository.GetAllAsync();

        var totalDirectRevenue = revenues.Sum(r => r.Amount);
        var totalSalesRevenue = sales.Sum(s => s.TotalAmount);
        var totalRevenue = totalDirectRevenue + totalSalesRevenue;
        var totalExpenses = expenses.Sum(e => e.Amount);
        var netProfit = totalRevenue - totalExpenses;
        var profitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100m : 0m;
        var totalSalesCount = sales.Count();
        var avgOrderValue = totalSalesCount > 0 ? totalSalesRevenue / totalSalesCount : 0m;

        return new ReportOverviewViewModel
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            ProfitMargin = Math.Round(profitMargin, 2),
            AverageOrderValue = Math.Round(avgOrderValue, 2),
            TotalCustomers = customers.Count(),
            TotalSales = totalSalesCount
        };
    }

    public async Task<RevenueReportViewModel> GetRevenueReportAsync(DateTime start, DateTime end)
    {
        var revenues = (await _revenueRepository.GetByDateRangeAsync(start, end)).ToList();
        var sales = (await _saleRepository.GetSalesByDateRangeAsync(start, end)).ToList();
        var customers = (await _customerRepository.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        var allRevenueItems = revenues.Select(r => new { r.Date, r.Amount, Source = r.Source, CustomerName = r.CustomerId.HasValue && customers.ContainsKey(r.CustomerId.Value) ? customers[r.CustomerId.Value] : "Direct Client" })
            .Concat(sales.Select(s => new { Date = s.SaleDate, Amount = s.TotalAmount, Source = "Direct Sales", CustomerName = s.Customer?.Name ?? (customers.ContainsKey(s.CustomerId) ? customers[s.CustomerId] : "Walk-in") }))
            .ToList();

        var byMonth = allRevenueItems
            .GroupBy(r => new { r.Date.Year, r.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyDataPoint
            {
                Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                Value = g.Sum(x => x.Amount)
            }).ToList();

        var bySource = allRevenueItems
            .GroupBy(r => string.IsNullOrWhiteSpace(r.Source) ? "General" : r.Source)
            .Select(g => new SourceSummary
            {
                Source = g.Key,
                Value = g.Sum(x => x.Amount)
            }).OrderByDescending(s => s.Value).ToList();

        var byCustomer = allRevenueItems
            .GroupBy(r => r.CustomerName)
            .Select(g => new CustomerRevenueSummary
            {
                CustomerName = g.Key,
                Value = g.Sum(x => x.Amount)
            }).OrderByDescending(c => c.Value).Take(10).ToList();

        return new RevenueReportViewModel
        {
            ByMonth = byMonth,
            BySource = bySource,
            ByCustomer = byCustomer
        };
    }

    public async Task<ExpenseReportViewModel> GetExpenseReportAsync(DateTime start, DateTime end)
    {
        var expenses = (await _expenseRepository.GetByDateRangeAsync(start, end)).ToList();

        var byCategory = expenses
            .GroupBy(e => string.IsNullOrWhiteSpace(e.Category) ? "Other" : e.Category)
            .Select(g => new ExpenseCategorySummary
            {
                Category = g.Key,
                Value = g.Sum(x => x.Amount)
            }).OrderByDescending(c => c.Value).ToList();

        var byMonth = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyDataPoint
            {
                Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                Value = g.Sum(x => x.Amount)
            }).ToList();

        var highestExpenses = expenses
            .OrderByDescending(e => e.Amount)
            .Take(10)
            .Select(e => new ExpenseTopItem
            {
                Label = $"{e.Category}: {e.Description ?? e.Vendor ?? "Expense"}",
                Value = e.Amount
            }).ToList();

        return new ExpenseReportViewModel
        {
            ByCategory = byCategory,
            ByMonth = byMonth,
            HighestExpenses = highestExpenses
        };
    }

    public async Task<CustomerGrowthViewModel> GetCustomerGrowthAsync(DateTime start, DateTime end)
    {
        var allCustomers = (await _customerRepository.GetAllAsync()).ToList();
        var totalCustomers = allCustomers.Count;
        var newCustomers = allCustomers.Count(c => c.RegistrationDate >= start && c.RegistrationDate <= end);
        var baseCount = totalCustomers - newCustomers;
        var growthRate = baseCount > 0 ? ((decimal)newCustomers / baseCount) * 100m : 100m;

        var monthlyGrowth = allCustomers
            .Where(c => c.RegistrationDate >= start && c.RegistrationDate <= end)
            .GroupBy(c => new { c.RegistrationDate.Year, c.RegistrationDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyDataPoint
            {
                Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                Value = g.Count()
            }).ToList();

        return new CustomerGrowthViewModel
        {
            TotalCustomers = totalCustomers,
            NewCustomers = newCustomers,
            GrowthRate = Math.Round(growthRate, 2),
            MonthlyGrowth = monthlyGrowth
        };
    }

    public async Task<ProductSalesReportViewModel> GetProductSalesReportAsync(DateTime start, DateTime end)
    {
        var sales = (await _saleRepository.GetAllWithDetailsAsync())
            .Where(s => s.SaleDate >= start && s.SaleDate <= end)
            .ToList();

        var productStats = sales.SelectMany(s => s.SaleItems)
            .GroupBy(si => new { si.ProductId, ProductName = si.Product?.Name ?? $"Product #{si.ProductId}", CostPrice = si.Product?.CostPrice ?? 0 })
            .Select(g =>
            {
                var qty = (int)g.Sum(x => x.Quantity);
                var rev = g.Sum(x => x.TotalPrice);
                var cost = qty * g.Key.CostPrice;
                return new ProductPerformanceItem
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = qty,
                    RevenueGenerated = rev,
                    ProductProfit = rev - cost
                };
            })
            .OrderByDescending(p => p.RevenueGenerated)
            .ToList();

        return new ProductSalesReportViewModel
        {
            TopProducts = productStats
        };
    }

    public async Task<MarketingReportViewModel> GetMarketingReportAsync(DateTime start, DateTime end)
    {
        var campaigns = (await _marketingRepository.GetByDateRangeAsync(start, end)).ToList();

        var summaries = campaigns.Select(c =>
        {
            var roi = c.ActualCost > 0 ? ((c.RevenueGenerated - c.ActualCost) / c.ActualCost) * 100m : 0m;
            return new MarketingCampaignSummary
            {
                Name = c.Name,
                Cost = c.ActualCost,
                Leads = c.Leads,
                Customers = c.CustomersAcquired,
                Revenue = c.RevenueGenerated,
                Roi = Math.Round(roi, 2)
            };
        }).OrderByDescending(c => c.Revenue).ToList();

        return new MarketingReportViewModel
        {
            Campaigns = summaries
        };
    }

    public async Task<byte[]> GenerateExcelReportAsync(string reportType, DateTime start, DateTime end)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        switch (reportType.ToLower())
        {
            case "revenue":
                var revReport = await GetRevenueReportAsync(start, end);
                worksheet.Cell(1, 1).Value = "Revenue Report";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;

                worksheet.Cell(3, 1).Value = "Source";
                worksheet.Cell(3, 2).Value = "Total Amount";
                worksheet.Row(3).Style.Font.Bold = true;

                int row = 4;
                foreach (var item in revReport.BySource)
                {
                    worksheet.Cell(row, 1).Value = item.Source;
                    worksheet.Cell(row, 2).Value = item.Value;
                    worksheet.Cell(row, 2).Style.NumberFormat.Format = "₹#,##0.00";
                    row++;
                }
                break;

            case "expense":
                var expReport = await GetExpenseReportAsync(start, end);
                worksheet.Cell(1, 1).Value = "Expense Report";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;

                worksheet.Cell(3, 1).Value = "Category";
                worksheet.Cell(3, 2).Value = "Total Expense";
                worksheet.Row(3).Style.Font.Bold = true;

                int expRow = 4;
                foreach (var item in expReport.ByCategory)
                {
                    worksheet.Cell(expRow, 1).Value = item.Category;
                    worksheet.Cell(expRow, 2).Value = item.Value;
                    worksheet.Cell(expRow, 2).Style.NumberFormat.Format = "₹#,##0.00";
                    expRow++;
                }
                break;

            case "profitloss":
            default:
                var overview = await GetOverviewAsync();
                worksheet.Cell(1, 1).Value = "Profit & Loss Statement";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;

                worksheet.Cell(3, 1).Value = "Metric";
                worksheet.Cell(3, 2).Value = "Amount";
                worksheet.Row(3).Style.Font.Bold = true;

                worksheet.Cell(4, 1).Value = "Total Revenue";
                worksheet.Cell(4, 2).Value = overview.TotalRevenue;
                worksheet.Cell(4, 2).Style.NumberFormat.Format = "₹#,##0.00";

                worksheet.Cell(5, 1).Value = "Total Expenses";
                worksheet.Cell(5, 2).Value = overview.TotalExpenses;
                worksheet.Cell(5, 2).Style.NumberFormat.Format = "₹#,##0.00";

                worksheet.Cell(6, 1).Value = "Net Profit";
                worksheet.Cell(6, 2).Value = overview.NetProfit;
                worksheet.Cell(6, 2).Style.NumberFormat.Format = "₹#,##0.00";

                worksheet.Cell(7, 1).Value = "Profit Margin (%)";
                worksheet.Cell(7, 2).Value = $"{overview.ProfitMargin}%";
                break;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
