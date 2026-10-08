using KPI_HUB.Models.ViewModels;
using KPI_HUB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI_HUB.Controllers;

[Authorize]
public class MarketingController : Controller
{
    private readonly IMarketingService _marketingService;

    public MarketingController(IMarketingService marketingService)
    {
        _marketingService = marketingService;
    }

    public async Task<IActionResult> Index()
    {
        var campaigns = await _marketingService.GetCampaignsAsync();
        return View(campaigns);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateMarketingCampaignViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMarketingCampaignViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _marketingService.AddCampaignAsync(model);
        TempData["SuccessMessage"] = "Marketing campaign created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _marketingService.GetCampaignForEditAsync(id);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditMarketingCampaignViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _marketingService.UpdateCampaignAsync(model);
        TempData["SuccessMessage"] = "Marketing campaign updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _marketingService.DeleteCampaignAsync(id);
        TempData["SuccessMessage"] = "Marketing campaign deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
