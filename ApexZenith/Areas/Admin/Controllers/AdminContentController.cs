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

    // ──────────────────────────────────────────────────────────────────────
    // Generic CRUD helpers
    //   The public actions below stay thin so MVC's action→view conventions
    //   keep working, while the shared create/edit/delete/list boilerplate
    //   lives here once instead of being copy-pasted per entity.
    // ──────────────────────────────────────────────────────────────────────
    private async Task<IActionResult> ListAsync<TEntity>(
        string viewName,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order)
        where TEntity : class
    {
        var model = await order(_context.Set<TEntity>().AsNoTracking()).ToListAsync();
        return View(viewName, model);
    }

    private async Task<IActionResult> CreateAsync<TEntity>(
        TEntity model, string viewName, string fallback, string? returnTo)
        where TEntity : class
    {
        var returnAction = ResolveReturnAction(returnTo, fallback);
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(viewName, model);
        }

        _context.Set<TEntity>().Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    private async Task<IActionResult> EditAsync<TEntity>(
        int id, TEntity model, string viewName, string fallback, string? returnTo,
        Action<TEntity, TEntity> apply)
        where TEntity : class
    {
        var returnAction = ResolveReturnAction(returnTo, fallback);
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnAction = returnAction;
            return View(viewName, model);
        }

        var item = await _context.Set<TEntity>().FindAsync(id);
        if (item == null) return NotFound();

        apply(item, model);
        await _context.SaveChangesAsync();
        return RedirectToAction(returnAction);
    }

    private async Task<IActionResult> DeleteConfirmedAsync<TEntity>(
        int id, string fallback, string? returnTo)
        where TEntity : class
    {
        var item = await _context.Set<TEntity>().FindAsync(id);
        if (item != null)
        {
            _context.Set<TEntity>().Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(ResolveReturnAction(returnTo, fallback));
    }

    // ──────────────────────────────────────────────────────────────────────
    // About
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> About()
        => await ListAsync<About>("About", q => q.OrderBy(x => x.Id));

    public IActionResult CreateAbout(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "About");
        return View(new About());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAbout(About model, string? returnTo = null)
        => await CreateAsync(model, "CreateAbout", "About", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<About>(id, model, "EditAbout", "About", returnTo, (e, m) =>
        {
            e.Title = m.Title;
            e.Content = m.Content;
            e.PhotoUrl = m.PhotoUrl;
        });
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
        => await DeleteConfirmedAsync<About>(id, "About", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Address
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Addresses()
        => await ListAsync<Address>("Addresses", q => q.OrderBy(x => x.Id));

    public IActionResult CreateAddress(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Addresses");
        return View(new Address());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAddress(Address model, string? returnTo = null)
        => await CreateAsync(model, "CreateAddress", "Addresses", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<Address>(id, model, "EditAddress", "Addresses", returnTo, (e, m) =>
        {
            e.IsHeadOffice = m.IsHeadOffice;
            e.Location = m.Location;
            e.Email = m.Email;
            e.Phone = m.Phone;
            e.GoogleMapUrl = m.GoogleMapUrl;
        });
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
        => await DeleteConfirmedAsync<Address>(id, "Addresses", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Services
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Services()
        => await ListAsync<Services>("Services", q => q.OrderBy(x => x.Id));

    public IActionResult CreateService(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Services");
        return View(new Services());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(Services model, string? returnTo = null)
        => await CreateAsync(model, "CreateService", "Services", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<Services>(id, model, "EditService", "Services", returnTo, (e, m) =>
        {
            e.Name = m.Name;
            e.Contents = m.Contents;
            e.PhotoUrl = m.PhotoUrl;
        });
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
        => await DeleteConfirmedAsync<Services>(id, "Services", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Clients
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Clients()
        => await ListAsync<Client>("Clients", q => q.OrderBy(x => x.Id));

    public IActionResult CreateClient(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Clients");
        return View(new Client());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClient(Client model, string? returnTo = null)
        => await CreateAsync(model, "CreateClient", "Clients", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<Client>(id, model, "EditClient", "Clients", returnTo, (e, m) =>
        {
            e.Name = m.Name;
            e.Logo = m.Logo;
            e.Website = m.Website;
        });
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
        => await DeleteConfirmedAsync<Client>(id, "Clients", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Team
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> TeamMembers()
        => await ListAsync<Team>("TeamMembers", q => q.OrderBy(x => x.Id));

    public IActionResult CreateTeam(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "TeamMembers");
        return View(new Team());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTeam(Team model, string? returnTo = null)
        => await CreateAsync(model, "CreateTeam", "TeamMembers", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<Team>(id, model, "EditTeam", "TeamMembers", returnTo, (e, m) =>
        {
            e.Name = m.Name;
            e.Position = m.Position;
            e.PhotoUrl = m.PhotoUrl;
            e.FacebookUrl = m.FacebookUrl;
            e.InstagramUrl = m.InstagramUrl;
            e.LinkedInUrl = m.LinkedInUrl;
        });
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
        => await DeleteConfirmedAsync<Team>(id, "TeamMembers", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Testimonials
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Testimonials()
        => await ListAsync<Testimonial>("Testimonials", q => q.OrderBy(x => x.Id));

    public IActionResult CreateTestimonial(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Testimonials");
        return View(new Testimonial());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTestimonial(Testimonial model, string? returnTo = null)
        => await CreateAsync(model, "CreateTestimonial", "Testimonials", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<Testimonial>(id, model, "EditTestimonial", "Testimonials", returnTo, (e, m) =>
        {
            e.Name = m.Name;
            e.Position = m.Position;
            e.Content = m.Content;
            e.PhotoUrl = m.PhotoUrl;
        });
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
        => await DeleteConfirmedAsync<Testimonial>(id, "Testimonials", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // News categories
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Categories()
        => await ListAsync<NewsCategory>("Categories", q => q.OrderBy(x => x.Id));

    public IActionResult CreateCategory(string? returnTo = null)
    {
        PrepareReturnAction(returnTo, "Categories");
        return View(new NewsCategory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(NewsCategory model, string? returnTo = null)
        => await CreateAsync(model, "CreateCategory", "Categories", returnTo);

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
        if (id != model.Id) return NotFound();
        return await EditAsync<NewsCategory>(id, model, "EditCategory", "Categories", returnTo, (e, m) =>
        {
            e.Name = m.Name;
        });
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
        => await DeleteConfirmedAsync<NewsCategory>(id, "Categories", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // News (uses a dedicated form model + category lookup, so kept explicit)
    // ──────────────────────────────────────────────────────────────────────
    public async Task<IActionResult> News()
        => await ListAsync<News>("News", q => q.OrderByDescending(x => x.Date));

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
        => await DeleteConfirmedAsync<News>(id, "News", returnTo);

    // ──────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────
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
