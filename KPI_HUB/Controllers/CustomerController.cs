using KPI_HUB.Models.ViewModels;
using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _customerService.GetCustomersAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerService.GetCustomerDetailsAsync(id);
        if (customer == null) return NotFound();

        return View(customer);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCustomerViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _customerService.AddCustomerAsync(model);
        TempData["SuccessMessage"] = "Customer added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerService.GetCustomerDetailsAsync(id);
        if (customer == null) return NotFound();

        var editModel = new EditCustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Status = customer.Status
        };

        return View(editModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCustomerViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _customerService.UpdateCustomerAsync(model);
        TempData["SuccessMessage"] = "Customer updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        TempData["SuccessMessage"] = "Customer deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
