using ApexZenith.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        returnUrl ??= Request.Query["ReturnUrl"].FirstOrDefault();
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["Title"] = "Sign in";
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["Title"] = "Sign in";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                //return Redirect(returnUrl);
                return Redirect("/");
            }

            return Redirect("/");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account is locked after too many failed attempts. Try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "The email or password you entered is not valid.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public IActionResult Register()
    {
        ViewData["Title"] = "Register staff account";
        return View(new RegisterStaffViewModel());
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterStaffViewModel model)
    {
        ViewData["Title"] = "Register An account";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        //if (!await _roleManager.RoleExistsAsync(model.RoleName))
        //{
        //    ModelState.AddModelError(nameof(model.RoleName), "That role does not exist.");
        //    return View(model);
        //}

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var create = await _userManager.CreateAsync(user, model.Password);
        if (!create.Succeeded)
        {
            foreach (var err in create.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        //await _userManager.AddToRoleAsync(user, model.RoleName);
        //TempData["StatusMessage"] = $"Account created for {model.Email} with role {model.RoleName}.";
        TempData["StatusMessage"] = $"Account created for {model.Email}.";

        return RedirectToAction(nameof(Register));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "Access denied";
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Admin", new { area = "Admin" });
    }
}
