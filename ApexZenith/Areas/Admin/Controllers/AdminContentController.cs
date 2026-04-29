using ApexZenith.Data;
using ApexZenith.Models;
using ApexZenith.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminContentController : Controller
{
    private readonly ApplicationDbContext _context;
    private static readonly Dictionary<string, string> AllowedReturnActions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Index"] = "Index",
        ["About"] = "About",
        ["Addresses"] = "Addresses",
        ["Services"] = "Services",
        ["Clients"] = "Clients",
        ["TeamMembers"] = "TeamMembers",
        ["Testimonials"] = "Testimonials",
        ["Categories"] = "Categories",
        ["News"] = "News"
    };

    public AdminContentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminContentOverviewViewModel
        {
            AboutCount = await _context.About.CountAsync(),
            AddressCount = await _context.Address.CountAsync(),
            ServicesCount = await _context.Services.CountAsync(),
            ClientsCount = await _context.Client.CountAsync(),
            TeamCount = await _context.Team.CountAsync(),
            TestimonialsCount = await _context.Testimonial.CountAsync(),
            CategoriesCount = await _context.NewsCategories.CountAsync(),
            NewsCount = await _context.News.CountAsync(),
            LastNewsDate = await _context.News
                .OrderByDescending(x => x.Date)
                .Select(x => (DateTime?)x.Date)
                .FirstOrDefaultAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> About()
    {
        var model = await _context.About
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Addresses()
    {
        var model = await _context.Address
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Services()
    {
        var model = await _context.Services
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Clients()
    {
        var model = await _context.Client
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> TeamMembers()
    {
        var model = await _context.Team
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Testimonials()
    {
        var model = await _context.Testimonial
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Categories()
    {
        var model = await _context.NewsCategories
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> News()
    {
        var model = await _context.News
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToListAsync();
        return View(model);
    }

    public IActionResult CreateAbout(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "About");
        return View(new About());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAbout(About model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "About");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.About.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditAbout(int id, string? returnTo = null)
    {
        var item = await _context.About.FindAsync(id);
        PrepareReturnAction(returnTo, "About");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAbout(int id, About model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "About");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        var item = await _context.About.FindAsync(id);
        if (item == null) return NotFound();

        item.Title = model.Title;
        item.Content = model.Content;
        item.PhotoUrl = model.PhotoUrl;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteAbout(int id, string? returnTo = null)
    {
        var item = await _context.About.FindAsync(id);
        PrepareReturnAction(returnTo, "About");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteAbout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAboutConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.About.FindAsync(id);
        if (item != null)
        {
            _context.About.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "About"));
    }

    public IActionResult CreateAddress(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Addresses");
        return View(new Address());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAddress(Address model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Addresses");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Address.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditAddress(int id, string? returnTo = null)
    {
        var item = await _context.Address.FindAsync(id);
        PrepareReturnAction(returnTo, "Addresses");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAddress(int id, Address model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Addresses");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        var item = await _context.Address.FindAsync(id);
        if (item == null) return NotFound();

        item.IsHeadOffice = model.IsHeadOffice;
        item.Location = model.Location;
        item.Email = model.Email;
        item.Phone = model.Phone;
        item.GoogleMapUrl = model.GoogleMapUrl;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteAddress(int id, string? returnTo = null)
    {
        var item = await _context.Address.FindAsync(id);
        PrepareReturnAction(returnTo, "Addresses");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteAddress")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAddressConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.Address.FindAsync(id);
        if (item != null)
        {
            _context.Address.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "Addresses"));
    }

    public IActionResult CreateService(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Services");
        return View(new Services());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(Services model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Services");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Services.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditService(int id, string? returnTo = null)
    {
        var item = await _context.Services.FindAsync(id);
        PrepareReturnAction(returnTo, "Services");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditService(int id, Services model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Services");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteService(int id, string? returnTo = null)
    {
        var item = await _context.Services.FindAsync(id);
        PrepareReturnAction(returnTo, "Services");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteService")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteServiceConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.Services.FindAsync(id);
        if (item != null)
        {
            _context.Services.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "Services"));
    }

    public IActionResult CreateClient(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Clients");
        return View(new Client());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClient(Client model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Clients");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Client.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditClient(int id, string? returnTo = null)
    {
        var item = await _context.Client.FindAsync(id);
        PrepareReturnAction(returnTo, "Clients");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClient(int id, Client model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Clients");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        var item = await _context.Client.FindAsync(id);
        if (item == null) return NotFound();

        item.Name = model.Name;
        item.Logo = model.Logo;
        item.Website = model.Website;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteClient(int id, string? returnTo = null)
    {
        var item = await _context.Client.FindAsync(id);
        PrepareReturnAction(returnTo, "Clients");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteClient")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClientConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.Client.FindAsync(id);
        if (item != null)
        {
            _context.Client.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "Clients"));
    }

    public IActionResult CreateTeam(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "TeamMembers");
        return View(new Team());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTeam(Team model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "TeamMembers");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Team.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditTeam(int id, string? returnTo = null)
    {
        var item = await _context.Team.FindAsync(id);
        PrepareReturnAction(returnTo, "TeamMembers");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTeam(int id, Team model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "TeamMembers");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        var item = await _context.Team.FindAsync(id);
        if (item == null) return NotFound();

        item.Name = model.Name;
        item.Position = model.Position;
        item.PhotoUrl = model.PhotoUrl;
        item.FacebookUrl = model.FacebookUrl;
        item.InstergramUrl = model.InstergramUrl;
        item.LinkedInUrl = model.LinkedInUrl;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteTeam(int id, string? returnTo = null)
    {
        var item = await _context.Team.FindAsync(id);
        PrepareReturnAction(returnTo, "TeamMembers");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteTeam")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTeamConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.Team.FindAsync(id);
        if (item != null)
        {
            _context.Team.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "TeamMembers"));
    }

    public IActionResult CreateTestimonial(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Testimonials");
        return View(new Testimonial());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTestimonial(Testimonial model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Testimonials");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Testimonial.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditTestimonial(int id, string? returnTo = null)
    {
        var item = await _context.Testimonial.FindAsync(id);
        PrepareReturnAction(returnTo, "Testimonials");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTestimonial(int id, Testimonial model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Testimonials");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        var item = await _context.Testimonial.FindAsync(id);
        if (item == null) return NotFound();

        item.Name = model.Name;
        item.Position = model.Position;
        item.Content = model.Content;
        item.PhotoUrl = model.PhotoUrl;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteTestimonial(int id, string? returnTo = null)
    {
        var item = await _context.Testimonial.FindAsync(id);
        PrepareReturnAction(returnTo, "Testimonials");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteTestimonial")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTestimonialConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.Testimonial.FindAsync(id);
        if (item != null)
        {
            _context.Testimonial.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "Testimonials"));
    }

    public IActionResult CreateCategory(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Categories");
        return View(new NewsCategory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(NewsCategory model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Categories");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.NewsCategories.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditCategory(int id, string? returnTo = null)
    {
        var item = await _context.NewsCategories.FindAsync(id);
        PrepareReturnAction(returnTo, "Categories");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, NewsCategory model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "Categories");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(model);
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteCategory(int id, string? returnTo = null)
    {
        var item = await _context.NewsCategories.FindAsync(id);
        PrepareReturnAction(returnTo, "Categories");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategoryConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.NewsCategories.FindAsync(id);
        if (item != null)
        {
            _context.NewsCategories.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "Categories"));
    }

    public IActionResult CreateNews(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "News");
        ViewBag.CategoryOptions = new SelectList(_context.NewsCategories.OrderBy(x => x.Name).AsNoTracking().ToList(), "Id", "Name");
        return View(new NewsFormModel { Date = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNews(NewsFormModel model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "News");
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
        ViewBag.CategoryOptions = new SelectList(await _context.NewsCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(), "Id", "Name", model.NewsCategoryId);
            return View(model);
        }

        var categoryName = await _context.NewsCategories
            .AsNoTracking()
            .Where(x => x.Id == model.NewsCategoryId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        var entity = new News
        {
            Headline = model.Headline,
            Content = model.Content,
            PhotoUrl = string.IsNullOrWhiteSpace(model.PhotoUrl) ? null : model.PhotoUrl.Trim(),
            Author = model.Author,
            PostedBy = categoryName ?? string.Empty,
            NewsCategoryId = model.NewsCategoryId,
            Date = EnsureUtc(model.Date == default ? DateTime.UtcNow : model.Date),
            PostedDate = EnsureUtc(model.Date == default ? DateTime.UtcNow : model.Date),
            NumberOfViews = 0,
            IsDeleted = false
        };

        _context.News.Add(entity);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> EditNews(int id, string? returnTo = null)
    {
        var item = await _context.News.FindAsync(id);
        PrepareReturnAction(returnTo, "News");
        ViewBag.CategoryOptions = new SelectList(await _context.NewsCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(), "Id", "Name", item?.NewsCategoryId);
        return item == null ? NotFound() : View(NewsFormModel.FromEntity(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditNews(int id, NewsFormModel model, string? returnTo = null)
    {
        var returnAction = ResolveReturnAction(returnTo, "News");
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            ViewBag.CategoryOptions = new SelectList(await _context.NewsCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(), "Id", "Name", model.NewsCategoryId);
            return View(model);
        }

        var item = await _context.News.FindAsync(id);
        if (item == null) return NotFound();

        var categoryName = await _context.NewsCategories
            .AsNoTracking()
            .Where(x => x.Id == model.NewsCategoryId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        item.Headline = model.Headline;
        item.Content = model.Content;
        item.PhotoUrl = string.IsNullOrWhiteSpace(model.PhotoUrl) ? null : model.PhotoUrl.Trim();
        item.Author = model.Author;
        item.PostedBy = categoryName ?? item.PostedBy;
        item.NewsCategoryId = model.NewsCategoryId;
        item.Date = EnsureUtc(model.Date == default ? item.Date : model.Date);
        item.PostedDate = item.Date;

        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    public async Task<IActionResult> DeleteNews(int id, string? returnTo = null)
    {
        var item = await _context.News.FindAsync(id);
        PrepareReturnAction(returnTo, "News");
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("DeleteNews")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteNewsConfirmed(int id, string? returnTo = null)
    {
        var item = await _context.News.FindAsync(id);
        if (item != null)
        {
            _context.News.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, "News"));
    }

    private static string ResolveReturnAction(string? returnTo, string fallback)
    {
        if (!AllowedReturnActions.TryGetValue(fallback, out var resolvedFallback))
        {
            resolvedFallback = "Index";
        }

        if (string.IsNullOrWhiteSpace(returnTo))
        {
            return resolvedFallback;
        }

        return AllowedReturnActions.TryGetValue(returnTo, out var resolvedAction)
            ? resolvedAction
            : resolvedFallback;
    }

    private void PrepareReturnAction(string? returnTo, string fallback)
    {
        ViewBag.ReturnAction = ResolveReturnAction(returnTo, fallback);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        if (value.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        return value.ToUniversalTime();
    }
}

public class AdminContentOverviewViewModel
{
    public int AboutCount { get; set; }
    public int AddressCount { get; set; }
    public int ServicesCount { get; set; }
    public int ClientsCount { get; set; }
    public int TeamCount { get; set; }
    public int TestimonialsCount { get; set; }
    public int CategoriesCount { get; set; }
    public int NewsCount { get; set; }
    public DateTime? LastNewsDate { get; set; }
}
