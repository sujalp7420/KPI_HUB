using KPI_HUB.Models.ViewModels;
using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize(Roles = "Admin,Manager,Viewer")]
public class SaleController : Controller
{
    private readonly ISaleService _saleService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;

    public SaleController(
        ISaleService saleService,
        ICustomerService customerService,
        IProductService productService)
    {
        _saleService = saleService;
        _customerService = customerService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _saleService.GetSalesAsync();
        return View(sales);
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await _saleService.GetSaleDetailAsync(id);
        if (sale == null) return NotFound();

        return View(sale);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _customerService.GetCustomersAsync();
        ViewBag.Products = await _productService.GetProductsAsync();
        return View(new CreateSaleViewModel { SaleDate = DateTime.UtcNow });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSaleViewModel model)
    {
        if (model.Items == null || model.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "At least one product item is required for a sale.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _customerService.GetCustomersAsync();
            ViewBag.Products = await _productService.GetProductsAsync();
            return View(model);
        }

        await _saleService.AddSaleAsync(model);
        TempData["SuccessMessage"] = "Sale recorded successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _saleService.DeleteSaleAsync(id);
        TempData["SuccessMessage"] = "Sale deleted and stock restored.";
        return RedirectToAction(nameof(Index));
    }
}
