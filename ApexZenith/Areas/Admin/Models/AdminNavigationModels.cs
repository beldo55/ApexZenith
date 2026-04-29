namespace ApexZenith.Areas.Admin.Models;

/// <summary>
/// One row in the admin sidebar. Maps to an MVC route (area + controller + action) or to a parent-only
/// folder row (see <see cref="IsSectionHeader"/>).
/// </summary>
public class AdminNavigationItem
{
    /// <summary>Text shown in the sidebar (use clear, user-facing language).</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Optional one-line hint for developers (shown on System Settings → Menu map tab only).</summary>
    public string? DeveloperDescription { get; init; }

    public string IconClass { get; init; } = "bi bi-circle";

    /// <summary>MVC area route token, usually <c>Admin</c>.</summary>
    public string Area { get; init; } = "Admin";

    /// <summary>MVC controller name without the <c>Controller</c> suffix (e.g. <c>AdminContent</c>).</summary>
    public string Controller { get; init; } = string.Empty;

    /// <summary>MVC action name (e.g. <c>Index</c>, <c>SystemSettings</c>).</summary>
    public string Action { get; init; } = "Index";

    /// <summary>Absolute path override. Prefer leaving null and using Area/Controller/Action/<see cref="SettingsTabId"/>.</summary>
    public string? Url { get; init; }

    /// <summary>When set, must match <c>?tab=</c> on the request so the correct child is highlighted under System Settings.</summary>
    public string? SettingsTabId { get; init; }

    /// <summary>Identity role names allowed to see this item. Empty = visible to all authenticated contexts we handle.</summary>
    public List<string> AllowedRoles { get; init; } = [];

    /// <summary>Nested links (sub-menu). Parent rows with children use <c>href="#"</c> in the layout for expand/collapse.</summary>
    public List<AdminNavigationItem> Children { get; init; } = [];

    /// <summary>True when this row only groups children (no target route).</summary>
    public bool IsSectionHeader =>
        Children.Count > 0 &&
        string.IsNullOrWhiteSpace(Controller) &&
        string.IsNullOrWhiteSpace(Action);

    /// <summary>True when the item points to a system settings tab instead of a regular action.</summary>
    public bool IsSettingsTab => !string.IsNullOrWhiteSpace(SettingsTabId);

    /// <summary>True when the item has a concrete route target.</summary>
    public bool HasTarget =>
        !string.IsNullOrWhiteSpace(Url) ||
        (!string.IsNullOrWhiteSpace(Controller) && !string.IsNullOrWhiteSpace(Action));

    /// <summary>Returns a normalized area token for comparisons.</summary>
    public string NormalizeArea() => (Area ?? string.Empty).Trim();

    /// <summary>Returns a normalized controller token for comparisons.</summary>
    public string NormalizeController() => (Controller ?? string.Empty).Trim();

    /// <summary>Returns a normalized action token for comparisons.</summary>
    public string NormalizeAction() => (Action ?? string.Empty).Trim();
}

public class AdminNavigationViewModel
{
    public IReadOnlyList<AdminNavigationItem> Items { get; init; } = Array.Empty<AdminNavigationItem>();
    public string CurrentArea { get; init; } = string.Empty;
    public string CurrentController { get; init; } = string.Empty;
    public string CurrentAction { get; init; } = string.Empty;

    /// <summary>True when the current request is inside the Admin area.</summary>
    public bool IsAdminArea =>
        string.Equals(CurrentArea, "Admin", StringComparison.OrdinalIgnoreCase);

    /// <summary>Checks whether the current request matches a navigation item.</summary>
    public bool IsActive(AdminNavigationItem item)
    {
        if (item is null)
        {
            return false;
        }

        return string.Equals(CurrentArea, item.NormalizeArea(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(CurrentController, item.NormalizeController(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(CurrentAction, item.NormalizeAction(), StringComparison.OrdinalIgnoreCase);
    }
}
