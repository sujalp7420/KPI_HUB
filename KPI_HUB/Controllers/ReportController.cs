using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize(Roles = "Admin,Manager,Viewer")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var overview = await _reportService.GetOverviewAsync();
        return View(overview);
    }

    public async Task<IActionResult> ProfitLoss(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-11);
        var endDate = end ?? DateTime.UtcNow;

        var overview = await _reportService.GetOverviewAsync();
        var revReport = await _reportService.GetRevenueReportAsync(startDate, endDate);
        var expReport = await _reportService.GetExpenseReportAsync(startDate, endDate);

        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        ViewBag.RevenueReport = revReport;
        ViewBag.ExpenseReport = expReport;

        return View(overview);
    }

    public async Task<IActionResult> Revenue(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-5);
        var endDate = end ?? DateTime.UtcNow;

        var report = await _reportService.GetRevenueReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        return View(report);
    }

    public async Task<IActionResult> Expense(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-5);
        var endDate = end ?? DateTime.UtcNow;

        var report = await _reportService.GetExpenseReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        return View(report);
    }

    public async Task<IActionResult> CustomerGrowth(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-11);
        var endDate = end ?? DateTime.UtcNow;

        var report = await _reportService.GetCustomerGrowthAsync(startDate, endDate);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        return View(report);
    }

    public async Task<IActionResult> ProductSales(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-11);
        var endDate = end ?? DateTime.UtcNow;

        var report = await _reportService.GetProductSalesReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        return View(report);
    }

    public async Task<IActionResult> Marketing(DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-11);
        var endDate = end ?? DateTime.UtcNow;

        var report = await _reportService.GetMarketingReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(string type = "profitloss", DateTime? start = null, DateTime? end = null)
    {
        var startDate = start ?? DateTime.UtcNow.AddMonths(-11);
        var endDate = end ?? DateTime.UtcNow;

        var fileBytes = await _reportService.GenerateExcelReportAsync(type, startDate, endDate);
        var fileName = $"{type}_report_{DateTime.UtcNow:yyyyMMdd}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
