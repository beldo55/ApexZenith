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

        // ?????????????????????????????????????????????????????????????????
        // FIX 1 (ghost-login guard) ?? Only redirect if the user is truly
        //         authenticated via Identity cookie, not via a Windows/IIS
        //         identity that snuck in. We check AuthenticationType so we
        //         don't accidentally redirect an IIS app-pool principal.
        // ?????????????????????????????????????????????????????????????????
        if (User.Identity?.IsAuthenticated == true
            && User.Identity.AuthenticationType == IdentityConstants.ApplicationScheme)
        {
            return RedirectToHome(returnUrl);
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

        var email = model.Email.Trim();

        // ?????????????????????????????????????????????????????????????????
        // FIX 2 ?? Resolve by email first, then sign in by UserName
        //
        //          PasswordSignInAsync(string userName, ...) looks up the
        //          user by UserName, not Email. As long as UserName == Email
        //          (which is true at creation time) it works. But if an admin
        //          ever edits a user's email through UsersManagementController
        //          the UserName stays the old value while Email changes, so
        //          login silently fails with "invalid credentials" even though
        //          the password is correct.
        //
        //          Resolving through FindByEmailAsync first guarantees we
        //          always sign in using the current UserName regardless of
        //          whether it still matches the Email.
        // ?????????????????????????????????????????????????????????????????
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal whether the email exists or not (enumeration guard)
            ModelState.AddModelError(string.Empty, "The email or password you entered is not valid.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToHome(returnUrl);
        }

        if (result.IsLockedOut)
        {
            // ?????????????????????????????????????????????????????????????
            // NOTE: Using the same generic message for lockout as for bad
            //       credentials prevents user enumeration (an attacker
            //       cannot tell whether the account exists by triggering a
            //       lockout message vs. an invalid-credentials message).
            // ?????????????????????????????????????????????????????????????
            ModelState.AddModelError(string.Empty, "The email or password you entered is not valid.");
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
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // ?????????????????????????????????????????????????????????????????????????
    // FIX 3 ?? Lock Register behind [Authorize(Roles = "Admin")]
    //
    //          Previously both GET and POST were [AllowAnonymous], meaning
    //          anyone on the internet could create an account with any role,
    //          including "Admin". Only an existing Admin should be able to
    //          create new staff accounts.
    // ?????????????????????????????????????????????????????????????????????????
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Register()
    {
        ViewData["Title"] = "Register staff account";
        return View(new RegisterStaffViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterStaffViewModel model)
    {
        ViewData["Title"] = "Register staff account";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        var roleName = string.IsNullOrWhiteSpace(model.RoleName) ? "User" : model.RoleName.Trim();

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            ModelState.AddModelError(nameof(model.RoleName), "That role does not exist.");
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
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

        await _userManager.AddToRoleAsync(user, roleName);
        TempData["StatusMessage"] = $"Account created for {email} with role {roleName}.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "Access denied";
        return View();
    }

    private IActionResult RedirectToHome(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home", new { area = "" });
    }
}
