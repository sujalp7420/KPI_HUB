using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _dashboardService.GetDashboardAsync();
        return View(dashboardData);
    }

    [HttpGet]
    public async Task<IActionResult> GetChartData()
    {
        var dashboardData = await _dashboardService.GetDashboardAsync();
        return Json(new
        {
            monthlyRevenueExpense = dashboardData.MonthlyRevenueExpense,
            monthlyProfit = dashboardData.MonthlyProfit,
            customerGrowth = dashboardData.CustomerGrowth,
            salesByProduct = dashboardData.SalesByProduct,
            expenseByCategory = dashboardData.ExpenseByCategory,
            marketingPerformance = dashboardData.MarketingPerformance
        });
    }
}
