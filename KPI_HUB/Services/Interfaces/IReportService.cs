using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IReportService
{
    Task<ReportOverviewViewModel> GetOverviewAsync();
    Task<RevenueReportViewModel> GetRevenueReportAsync(DateTime start, DateTime end);
    Task<ExpenseReportViewModel> GetExpenseReportAsync(DateTime start, DateTime end);
    Task<CustomerGrowthViewModel> GetCustomerGrowthAsync(DateTime start, DateTime end);
    Task<ProductSalesReportViewModel> GetProductSalesReportAsync(DateTime start, DateTime end);
    Task<MarketingReportViewModel> GetMarketingReportAsync(DateTime start, DateTime end);
    Task<byte[]> GenerateExcelReportAsync(string reportType, DateTime start, DateTime end);
}
