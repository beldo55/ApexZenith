using ApexZenith.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RoleAssignmentsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleAssignmentsController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var roles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name ?? string.Empty).ToListAsync();
        var model = new RoleAssignmentIndexViewModel
        {
            Roles = roles,
            Users = new List<RoleAssignmentUserRowViewModel>()
        };
        foreach (var user in users)
        {
            model.Users.Add(new RoleAssignmentUserRowViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? string.Empty,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !await _roleManager.RoleExistsAsync(roleName))
        {
            TempData["ErrorMessage"] = "User or role not found.";
            return RedirectToAction(nameof(Index));
        }

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        TempData["StatusMessage"] = $"Added {user.Email ?? user.UserName} to {roleName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !await _roleManager.RoleExistsAsync(roleName))
        {
            TempData["ErrorMessage"] = "User or role not found.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.RemoveFromRoleAsync(user, roleName);
        TempData["StatusMessage"] = $"Removed {user.Email ?? user.UserName} from {roleName}.";
        return RedirectToAction(nameof(Index));
    }
}
