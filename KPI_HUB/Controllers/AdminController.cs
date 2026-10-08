using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Controllers;

[Authorize(Roles = "Admin,Founder")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<AdminUserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new AdminUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt
            });
        }

        return View(userViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> ManageRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        var model = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = allRoles.Select(r => new RoleSelectionItem
            {
                RoleName = r.Name ?? string.Empty,
                Selected = userRoles.Contains(r.Name ?? string.Empty)
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.RoleName).ToList();

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Failed to remove previous roles.");
            return View(model);
        }

        var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
        if (!addResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Failed to add selected roles.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Roles updated successfully for {user.Email}.";
        return RedirectToAction(nameof(Index));
    }
}
