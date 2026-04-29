using ApexZenith.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RolesManagementController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RolesManagementController(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var roleEntities = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        var roles = new List<RoleManagementRowViewModel>();
        foreach (var role in roleEntities)
        {
            roles.Add(new RoleManagementRowViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UserCount = (await _userManager.GetUsersInRoleAsync(role.Name ?? string.Empty)).Count
            });
        }

        return View(new RoleManagementIndexViewModel { Roles = roles });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleName)
    {
        roleName = roleName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            TempData["ErrorMessage"] = "Role name is required.";
            return RedirectToAction(nameof(Index));
        }

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            TempData["ErrorMessage"] = "That role already exists.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        TempData[result.Succeeded ? "StatusMessage" : "ErrorMessage"] = result.Succeeded
            ? $"Role '{roleName}' created."
            : string.Join("; ", result.Errors.Select(e => e.Description));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
        {
            TempData["ErrorMessage"] = "Role not found.";
            return RedirectToAction(nameof(Index));
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
        {
            TempData["ErrorMessage"] = "Remove users from the role first.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _roleManager.DeleteAsync(role);
        TempData[result.Succeeded ? "StatusMessage" : "ErrorMessage"] = result.Succeeded
            ? $"Role '{role.Name}' deleted."
            : string.Join("; ", result.Errors.Select(e => e.Description));
        return RedirectToAction(nameof(Index));
    }
}
