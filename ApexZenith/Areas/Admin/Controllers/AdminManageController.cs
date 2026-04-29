using ApexZenith.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminManageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminManageController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminManageIndexViewModel
        {
            About = await _context.About.AsNoTracking().ToListAsync(),
            Address = await _context.Address.AsNoTracking().ToListAsync(),
            Services = await _context.Services.AsNoTracking().ToListAsync(),
            Clients = await _context.Client.AsNoTracking().ToListAsync(),
            Team = await _context.Team.AsNoTracking().ToListAsync(),
            Testimonials = await _context.Testimonial.AsNoTracking().ToListAsync(),
            Categories = await _context.NewsCategories.AsNoTracking().ToListAsync(),
            News = await _context.News.AsNoTracking().OrderByDescending(x => x.Date).ToListAsync(),
            Contacts = await _context.Contact.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> SystemSettings(string? tab = null)
    {
        var model = new AdminSystemSettingsViewModel
        {
            ActiveTab = NormalizeTab(tab),
            UserCount = await _userManager.Users.CountAsync(),
            RoleCount = await _roleManager.Roles.CountAsync(),
            AdminUserCount = await _userManager.GetUsersInRoleAsync("Admin").ContinueWith(t => t.Result.Count),
            LastNewsDate = await _context.News
                .OrderByDescending(x => x.Date)
                .Select(x => (DateTime?)x.Date)
                .FirstOrDefaultAsync(),
        };

        return View(model);
    }

    private static string NormalizeTab(string? tab)
    {
        if (string.IsNullOrWhiteSpace(tab))
        {
            return "general";
        }

        tab = tab.ToLowerInvariant();
        return tab is "general" or "resources"
            ? tab
            : "general";
    }
}

public class AdminManageIndexViewModel
{
    public List<ApexZenith.Models.About> About { get; set; } = new();
    public List<ApexZenith.Models.Address> Address { get; set; } = new();
    public List<ApexZenith.Models.Services> Services { get; set; } = new();
    public List<ApexZenith.Models.Client> Clients { get; set; } = new();
    public List<ApexZenith.Models.Team> Team { get; set; } = new();
    public List<ApexZenith.Models.Testimonial> Testimonials { get; set; } = new();
    public List<ApexZenith.Models.NewsCategory> Categories { get; set; } = new();
    public List<ApexZenith.Models.News> News { get; set; } = new();
    public List<ApexZenith.Models.Contact> Contacts { get; set; } = new();
}

public class AdminSystemSettingsViewModel
{
    public string ActiveTab { get; set; } = "general";
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public int AdminUserCount { get; set; }
    public DateTime? LastNewsDate { get; set; }
}
