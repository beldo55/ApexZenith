using ApexZenith.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
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
        ViewData["Title"] = "Dashboard";

        var model = new AdminDashboardViewModel
        {
            AboutCount = await _context.About.CountAsync(),
            AddressCount = await _context.Address.CountAsync(),
            ServicesCount = await _context.Services.CountAsync(),
            ClientCount = await _context.Client.CountAsync(),
            TeamCount = await _context.Team.CountAsync(),
            TestimonialCount = await _context.Testimonial.CountAsync(),
            NewsCount = await _context.News.CountAsync(),
            CategoriesCount = await _context.NewsCategories.CountAsync(),
            UserCount = await _userManager.Users.CountAsync(),
            RoleCount = await _roleManager.Roles.CountAsync(),
            AdminUserCount = await _userManager.GetUsersInRoleAsync("Admin").ContinueWith(t => t.Result.Count),
            LatestServices = await _context.Services
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToListAsync(),
            LatestNews = await _context.News
                .OrderByDescending(x => x.Date)
                .Take(5)
                .ToListAsync(),
            LatestContacts = await _context.Contact
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }
}

public class AdminDashboardViewModel
{
    public int AboutCount { get; set; }
    public int AddressCount { get; set; }
    public int ServicesCount { get; set; }
    public int ClientCount { get; set; }
    public int TeamCount { get; set; }
    public int TestimonialCount { get; set; }
    public int NewsCount { get; set; }
    public int CategoriesCount { get; set; }
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public int AdminUserCount { get; set; }
    public List<ApexZenith.Models.Services> LatestServices { get; set; } = new();
    public List<ApexZenith.Models.News> LatestNews { get; set; } = new();
    public List<ApexZenith.Models.Contact> LatestContacts { get; set; } = new();
}
