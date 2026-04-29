using System.ComponentModel.DataAnnotations;
using ApexZenith.Areas.Admin.Models;
using ApexZenith.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminResourceController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminResourceController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Navigation resources";

        var items = await _db.Resource
            .AsNoTracking()
            .Include(x => x.RoleRules)
            .OrderBy(x => x.ParentId == null ? 0 : 1)
            .ThenBy(x => x.ParentId)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Id)
            .ToListAsync();

        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add menu item";
        await PopulateParentSelectAsync(null);
        await PopulateRoleOptionsAsync();
        return View(new ResourceFormModel { Area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResourceFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateParentSelectAsync(model.ParentId);
            await PopulateRoleOptionsAsync();
            return View(model);
        }

        var entity = model.ToEntity();
        _db.Resource.Add(entity);
        await _db.SaveChangesAsync();

        await SyncRolesAsync(entity.Id, model.SelectedRoles);

        TempData["StatusMessage"] = "Menu item created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.Resource.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        // Manual fetch of roles since ResourceRole doesn't have a direct nav property in the 'clean' model
        var roles = await _db.ResourceRoles
            .Where(x => x.ResourceId == id)
            .Select(x => x.RoleId)
            .ToListAsync();

        await PopulateParentSelectAsync(entity.ParentId, excludeId: id);
        await PopulateRoleOptionsAsync();
        var model = ResourceFormModel.FromEntity(entity);
        model.SelectedRoles = roles;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ResourceFormModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateParentSelectAsync(model.ParentId, excludeId: id);
            await PopulateRoleOptionsAsync();
            return View(model);
        }

        var entity = await _db.Resource.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        if (model.ParentId == id)
        {
            ModelState.AddModelError(nameof(model.ParentId), "An item cannot be its own parent.");
            await PopulateParentSelectAsync(model.ParentId, excludeId: id);
            return View(model);
        }

        model.ApplyTo(entity);
        await _db.SaveChangesAsync();
        await SyncRolesAsync(entity.Id, model.SelectedRoles);

        TempData["StatusMessage"] = "Menu item updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Resource
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return NotFound();

        var hasChildren = await _db.Resource.AnyAsync(x => x.ParentId == id);
        if (hasChildren)
        {
            TempData["ErrorMessage"] = "Delete the child items first.";
            return RedirectToAction(nameof(Index));
        }

        var roles = await _db.ResourceRoles
            .AsNoTracking()
            .Where(x => x.ResourceId == id)
            .Select(x => x.RoleId)
            .ToListAsync();

        var model = ResourceFormModel.FromEntity(entity);
        model.SelectedRoles = roles;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _db.Resource.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        var hasChildren = await _db.Resource.AnyAsync(x => x.ParentId == id);
        if (hasChildren)
        {
            TempData["ErrorMessage"] = "Delete the child items first.";
            return RedirectToAction(nameof(Index));
        }

        var existingRoles = await _db.ResourceRoles.Where(x => x.ResourceId == id).ToListAsync();
        _db.ResourceRoles.RemoveRange(existingRoles);
        _db.Resource.Remove(entity);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = "Menu item deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SyncRolesAsync(int resourceId, List<string> selected)
    {
        var existing = await _db.ResourceRoles.Where(x => x.ResourceId == resourceId).ToListAsync();
        _db.ResourceRoles.RemoveRange(existing);

        foreach (var r in selected.Distinct().Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            _db.ResourceRoles.Add(new ResourceRole
            {
                ResourceId = resourceId,
                RoleId = r.Trim()
            });
        }
        await _db.SaveChangesAsync();
    }

    private async Task PopulateParentSelectAsync(int? currentParentId, int? excludeId = null)
    {
        var options = await _db.Resource
            .AsNoTracking()
            .Where(x => excludeId == null || x.Id != excludeId.Value)
            .OrderBy(x => x.Order)
            .Select(x => new MenuParentOption(x.Id, x.Name))
            .ToListAsync();

        ViewBag.ParentOptions = options;
        ViewBag.CurrentParentId = currentParentId;
    }

    private async Task PopulateRoleOptionsAsync()
    {
        ViewBag.RoleOptions = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => r.Name ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToListAsync();
    }
}
