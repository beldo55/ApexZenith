using ApexZenith.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ─────────────────────────────────────────────────────────────────────────────
// FIX 1 ── Disable IIS Windows Authentication auto-login
//           ROOT CAUSE of "user appears logged in on a brand-new browser".
//           IIS/site4now runs Windows Auth by default and populates
//           HttpContext.User with the app-pool identity, making
//           User.Identity.IsAuthenticated == true for every request even when
//           no one has logged in.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

// ─────────────────────────────────────────────────────────────────────────────
// FIX 2 ── ForwardedHeaders: only trust the actual reverse proxy
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.KnownProxies.Add(IPAddress.Loopback);
    options.KnownProxies.Add(IPAddress.IPv6Loopback);
    // If site4now's proxy has a fixed IP, add it here:
    // options.KnownProxies.Add(IPAddress.Parse("x.x.x.x"));
});

// ─────────────────────────────────────────────────────────────────────────────
// FIX 3 ── DataProtection: scope keys to the environment so dev cookies
//           can never be accepted in production and vice versa.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")))
    .SetApplicationName($"ApexZenith-{builder.Environment.EnvironmentName}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Account/Login";
    options.LogoutPath = "/Admin/Account/Logout";
    options.AccessDeniedPath = "/Admin/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "ApexZenith.Auth";
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ForwardedHeaders MUST come before everything else in the pipeline
app.UseForwardedHeaders();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var configuration = services.GetRequiredService<IConfiguration>();
    var environment = services.GetRequiredService<IHostEnvironment>();

    await context.Database.MigrateAsync();

    await SeedData.SeedRolesAsync(roleManager);
    await SeedData.SeedAdminNavigationMenuAsync(context);
    await SeedData.InitializeAsync(context);

    // FIX 6 ── Re-enable bootstrap admin seeding (was commented out)
    //           Reads BootstrapAdmin:Email + BootstrapAdmin:Password from config.
    //           Only runs when AspNetUsers table is empty, safe to leave on.
    await SeedData.EnsureBootstrapAdminAsync(userManager, roleManager, configuration, environment);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();