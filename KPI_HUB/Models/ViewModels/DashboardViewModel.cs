namespace KPI_HUB.Models.ViewModels;

public class DashboardViewModel
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalSales { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal MarketingRoi { get; set; }
    public List<MonthlyTrendItem> MonthlyRevenueExpense { get; set; } = new();
    public List<MonthlyProfitItem> MonthlyProfit { get; set; } = new();
    public List<CustomerGrowthItem> CustomerGrowth { get; set; } = new();
    public List<ProductSalesItem> SalesByProduct { get; set; } = new();
    public List<ExpenseCategoryItem> ExpenseByCategory { get; set; } = new();
    public List<MarketingPerformanceItem> MarketingPerformance { get; set; } = new();
    public List<InsightItem> Insights { get; set; } = new();
    public List<ProductListViewModel> LowStockProducts { get; set; } = new();
}

public class MonthlyTrendItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
}

public class MonthlyProfitItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Profit { get; set; }
}

public class CustomerGrowthItem
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProductSalesItem
{
    public string Label { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class ExpenseCategoryItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class MarketingPerformanceItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
}

public class InsightItem
{
    public string Message { get; set; } = string.Empty;
    public string Tone { get; set; } = "info";
}

public class CustomerListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPurchases { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class CustomerDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPurchases { get; set; }
    public List<CustomerPurchaseHistoryItem> PurchaseHistory { get; set; } = new();
}

public class CustomerPurchaseHistoryItem
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateCustomerViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "Active";
}

public class EditCustomerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "Active";
}

public class ProductListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStock { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ProfitPerProduct => Price - CostPrice;
}

public class ProductFormViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStock { get; set; }
    public string Status { get; set; } = "Active";
}

public class SaleListViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class SaleDetailViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<SaleItemViewModel> Items { get; set; } = new();
}

public class SaleItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateSaleViewModel
{
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; } = "Card";
    public string Status { get; set; } = "Completed";
    public List<CreateSaleItemViewModel> Items { get; set; } = new();
}

public class CreateSaleItemViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class RevenueListViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CustomerName { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class CreateRevenueViewModel
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CustomerId { get; set; }
    public string PaymentMethod { get; set; } = "Bank Transfer";
    public string? ReferenceNumber { get; set; }
}

public class EditRevenueViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CustomerId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
}

public class ExpenseListViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Vendor { get; set; }
}

public class CreateExpenseViewModel
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PaymentMethod { get; set; } = "Bank Transfer";
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
}

public class EditExpenseViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
}

public class MarketingCampaignListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Platform { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int Leads { get; set; }
    public int CustomersAcquired { get; set; }
    public decimal RevenueGenerated { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Roi => ActualCost == 0 ? 0 : ((RevenueGenerated - ActualCost) / ActualCost) * 100m;
}

public class CreateMarketingCampaignViewModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public string Platform { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int Leads { get; set; }
    public int CustomersAcquired { get; set; }
    public decimal RevenueGenerated { get; set; }
    public string Status { get; set; } = "Active";
}

public class EditMarketingCampaignViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Platform { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int Leads { get; set; }
    public int CustomersAcquired { get; set; }
    public decimal RevenueGenerated { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class EmployeeListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateEmployeeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}

public class EditEmployeeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ReportOverviewViewModel
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalSales { get; set; }
}

public class RevenueReportViewModel
{
    public List<MonthlyDataPoint> ByMonth { get; set; } = new();
    public List<SourceSummary> BySource { get; set; } = new();
    public List<CustomerRevenueSummary> ByCustomer { get; set; } = new();
}

public class ExpenseReportViewModel
{
    public List<ExpenseCategorySummary> ByCategory { get; set; } = new();
    public List<MonthlyDataPoint> ByMonth { get; set; } = new();
    public List<ExpenseTopItem> HighestExpenses { get; set; } = new();
}

public class CustomerGrowthViewModel
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public decimal GrowthRate { get; set; }
    public List<MonthlyDataPoint> MonthlyGrowth { get; set; } = new();
}

public class ProductSalesReportViewModel
{
    public List<ProductPerformanceItem> TopProducts { get; set; } = new();
}

public class ProductPerformanceItem
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal RevenueGenerated { get; set; }
    public decimal ProductProfit { get; set; }
}

public class MarketingReportViewModel
{
    public List<MarketingCampaignSummary> Campaigns { get; set; } = new();
}

public class MonthlyDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class SourceSummary
{
    public string Source { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class CustomerRevenueSummary
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class ExpenseCategorySummary
{
    public string Category { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class ExpenseTopItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class MarketingCampaignSummary
{
    public string Name { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Leads { get; set; }
    public int Customers { get; set; }
    public decimal Revenue { get; set; }
    public decimal Roi { get; set; }
}
