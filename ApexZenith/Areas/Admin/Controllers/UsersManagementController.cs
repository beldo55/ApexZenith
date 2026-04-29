using ApexZenith.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersManagementController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersManagementController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = await BuildIndexViewModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(UserManagementEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await RehydrateEditModelAsync(model);
            return View("Index", await BuildIndexViewModelAsync(model));
        }

        var email = model.Email.Trim();
        var user = string.IsNullOrWhiteSpace(model.Id)
            ? null
            : await _userManager.FindByIdAsync(model.Id);

        if (user is null)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required for new users.");
                model = await RehydrateEditModelAsync(model);
                return View("Index", await BuildIndexViewModelAsync(model));
            }

            if (await _userManager.FindByEmailAsync(email) is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
                model = await RehydrateEditModelAsync(model);
                return View("Index", await BuildIndexViewModelAsync(model));
            }

            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = model.PhoneNumber?.Trim(),
                EmailConfirmed = model.EmailConfirmed
            };

            var createResult = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);
                model = await RehydrateEditModelAsync(model);
                return View("Index", await BuildIndexViewModelAsync(model));
            }
        }
        else
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null && existingUser.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Another account already uses that email address.");
                model = await RehydrateEditModelAsync(model);
                return View("Index", await BuildIndexViewModelAsync(model));
            }

            user.Email = email;
            user.UserName = email;
            user.PhoneNumber = model.PhoneNumber?.Trim();
            user.EmailConfirmed = model.EmailConfirmed;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);
                model = await RehydrateEditModelAsync(model);
                return View("Index", await BuildIndexViewModelAsync(model));
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!passwordResult.Succeeded)
                {
                    AddIdentityErrors(passwordResult);
                    model = await RehydrateEditModelAsync(model);
                    return View("Index", await BuildIndexViewModelAsync(model));
                }
            }
        }

        TempData["StatusMessage"] = user is null ? "User created." : "User updated.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        if (string.Equals(user.UserName, user.Email, StringComparison.OrdinalIgnoreCase)
            && await _userManager.GetRolesAsync(user) is { Count: > 0 } roles
            && roles.Contains("Admin", StringComparer.OrdinalIgnoreCase)
            && (await _userManager.GetUsersInRoleAsync("Admin")).Count == 1)
        {
            TempData["ErrorMessage"] = "You cannot delete the last Admin user.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        TempData[result.Succeeded ? "StatusMessage" : "ErrorMessage"] = result.Succeeded
            ? "User deleted."
            : string.Join("; ", result.Errors.Select(e => e.Description));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Json(new
        {
            id = user.Id,
            email = user.Email ?? string.Empty,
            phoneNumber = user.PhoneNumber ?? string.Empty,
            emailConfirmed = user.EmailConfirmed,
            roles = await _userManager.GetRolesAsync(user)
        });
    }

    private async Task<UserManagementIndexViewModel> BuildIndexViewModelAsync(UserManagementEditViewModel? editModel = null)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var rows = new List<UserManagementRowViewModel>();
        foreach (var user in users)
        {
            rows.Add(new UserManagementRowViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
            });
        }

        return new UserManagementIndexViewModel
        {
            Users = rows,
            EditModel = editModel ?? new UserManagementEditViewModel()
        };
    }

    private async Task<UserManagementEditViewModel> RehydrateEditModelAsync(UserManagementEditViewModel model, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return model;
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return model;
        }

        model.Id = user.Id;
        model.Email = user.Email ?? string.Empty;
        model.PhoneNumber = user.PhoneNumber;
        model.EmailConfirmed = user.EmailConfirmed;
        model.Password = string.Empty;
        model.ConfirmPassword = string.Empty;
        return model;
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
