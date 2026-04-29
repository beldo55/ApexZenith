namespace ApexZenith.Areas.Admin.Models;

/// <summary>
/// One row in the admin sidebar menu, stored in the database for resource/navigation management.
/// </summary>
public class Resource
{
    public int Id { get; set; }

    public int? ParentId { get; set; }
    public Resource? Parent { get; set; }
    public ICollection<Resource> Children { get; set; } = [];

    public string Name { get; set; } = string.Empty;
    public string? DeveloperNote { get; set; }

    public string Area { get; set; } = "Admin";
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public bool IsAction { get; set; } = true;

    public string? IconClass { get; set; }

    public int Order { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public string? SettingsTabId { get; set; }



    public ICollection<ResourceRole> RoleRules { get; set; } = [];
}

/// <summary>Associates a menu row with an Identity role name (e.g. Admin, User).</summary>
public class ResourceRole
{
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public string RoleId { get; set; } = string.Empty;
}
