using KPI_HUB.Models.ViewModels;
using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index(string? category)
    {
        var expenses = await _expenseService.GetExpensesAsync();
        if (!string.IsNullOrEmpty(category))
        {
            expenses = expenses.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.SelectedCategory = category;
        return View(expenses);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateExpenseViewModel { Date = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _expenseService.AddExpenseAsync(model);
        TempData["SuccessMessage"] = "Expense recorded successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _expenseService.GetExpenseForEditAsync(id);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditExpenseViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _expenseService.UpdateExpenseAsync(model);
        TempData["SuccessMessage"] = "Expense updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _expenseService.DeleteExpenseAsync(id);
        TempData["SuccessMessage"] = "Expense deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
