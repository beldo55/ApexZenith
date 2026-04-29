using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Amaincompany.Data;

public static partial class SeedData
{
    /// <summary>
    /// Creates the first administrator when the user store is empty (typical dev bootstrap).
    /// Runs only in Development unless <c>BootstrapAdmin:Force</c> is true.
    /// </summary>
    public static async Task EnsureBootstrapAdminAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var force = configuration.GetValue("BootstrapAdmin:Force", false);
        if (!environment.IsDevelopment() && !force)
        {
            return;
        }

        var email = configuration["BootstrapAdmin:Email"] ?? "admin@maincompany.local";
        var password = configuration["BootstrapAdmin:Password"];
        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant()
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Bootstrap admin user could not be created: " +
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        await userManager.AddToRoleAsync(user, "Admin");
    }
}
