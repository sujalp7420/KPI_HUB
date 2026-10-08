using KPI_HUB.Models.ViewModels;
using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize]
public class RevenueController : Controller
{
    private readonly IRevenueService _revenueService;
    private readonly ICustomerService _customerService;

    public RevenueController(IRevenueService revenueService, ICustomerService customerService)
    {
        _revenueService = revenueService;
        _customerService = customerService;
    }

    public async Task<IActionResult> Index()
    {
        var revenues = await _revenueService.GetRevenueAsync();
        return View(revenues);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _customerService.GetCustomersAsync();
        return View(new CreateRevenueViewModel { Date = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRevenueViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _customerService.GetCustomersAsync();
            return View(model);
        }

        await _revenueService.AddRevenueAsync(model);
        TempData["SuccessMessage"] = "Revenue entry recorded successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _revenueService.GetRevenueForEditAsync(id);
        if (model == null) return NotFound();

        ViewBag.Customers = await _customerService.GetCustomersAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditRevenueViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _customerService.GetCustomersAsync();
            return View(model);
        }

        await _revenueService.UpdateRevenueAsync(model);
        TempData["SuccessMessage"] = "Revenue entry updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _revenueService.DeleteRevenueAsync(id);
        TempData["SuccessMessage"] = "Revenue entry deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
