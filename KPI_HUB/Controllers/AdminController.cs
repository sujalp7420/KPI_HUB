using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Controllers;

[Authorize(Roles = "Admin")]
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

        var rolesList = await _roleManager.Roles.ToListAsync();
        var roleViewModels = new List<RoleViewModel>();

        foreach (var r in rolesList)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(r.Name!);
            string desc = r.Name switch
            {
                "Admin" => "Full access to the application, including managing users and roles.",
                "Manager" => "Can manage business records and view reports.",
                "Viewer" => "Can view dashboards, campaigns, leads, and reports but cannot create, edit, or delete any records.",
                _ => "Custom role permissions"
            };

            roleViewModels.Add(new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = desc,
                UserCount = usersInRole.Count
            });
        }

        var model = new AdminIndexViewModel
        {
            Users = userViewModels,
            Roles = roleViewModels
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields to create a user.";
            return RedirectToAction(nameof(Index));
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            TempData["ErrorMessage"] = "A user with this email address already exists.";
            return RedirectToAction(nameof(Index));
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            TempData["SuccessMessage"] = $"User '{model.FullName}' created successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid user details provided.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"User details updated for {user.Email}.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserRoles(string userId, List<string> selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        selectedRoles ??= new List<string>();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            TempData["ErrorMessage"] = "Failed to update user roles.";
            return RedirectToAction(nameof(Index));
        }

        if (selectedRoles.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!addResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to assign selected roles.";
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["SuccessMessage"] = $"Roles updated successfully for {user.Email}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.RoleName))
        {
            TempData["ErrorMessage"] = "Role name cannot be empty.";
            return RedirectToAction(nameof(Index));
        }

        var roleName = model.RoleName.Trim();
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            TempData["ErrorMessage"] = $"Role '{roleName}' already exists.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Role '{roleName}' created successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var currentUserId = _userManager.GetUserId(User);
        if (currentUserId == id)
        {
            TempData["ErrorMessage"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"User {user.Email} deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ManageRoles(string? id)
    {
        return RedirectToAction(nameof(Index));
    }
}
