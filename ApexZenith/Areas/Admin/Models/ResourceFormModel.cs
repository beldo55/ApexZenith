using System.ComponentModel.DataAnnotations;

namespace ApexZenith.Areas.Admin.Models
{
    public record MenuParentOption(int Id, string Name);

    public class ResourceFormModel
    {
        public int Id { get; set; }

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(400)]
        public string? DeveloperNote { get; set; }

        [Required]
        [MaxLength(80)]
        public string Area { get; set; } = "Admin";

        [MaxLength(120)]
        public string? Controller { get; set; }

        [MaxLength(120)]
        public string? Action { get; set; }

        [Display(Name = "Is action")]
        public bool IsAction { get; set; } = true;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [MaxLength(120)]
        [Display(Name = "Tab key")]
        public string? SettingsTabId { get; set; }

        [Required]
        [MaxLength(120)]
        [Display(Name = "Icon class")]
        public string IconClass { get; set; } = "bi bi-circle";

        [Display(Name = "Order")]
        public int Order { get; set; }

        /// <summary>
        /// Selected roles for this resource
        /// </summary>
        public List<string> SelectedRoles { get; set; } = [];

        public static ResourceFormModel FromEntity(Resource e) => new()
        {
            Id = e.Id,
            ParentId = e.ParentId,
            Name = e.Name,
            DeveloperNote = e.DeveloperNote,
            Area = e.Area,
            Controller = e.Controller,
            Action = e.Action,
            IsAction = e.IsAction,
            IsActive = e.IsActive,
            SettingsTabId = e.SettingsTabId,
            IconClass = string.IsNullOrWhiteSpace(e.IconClass) ? "bi bi-circle" : e.IconClass,
            Order = e.Order
        };

        public Resource ToEntity() => new()
        {
            ParentId = ParentId,
            Name = Name.Trim(),
            DeveloperNote = string.IsNullOrWhiteSpace(DeveloperNote) ? null : DeveloperNote.Trim(),
            Area = Area.Trim(),
            Controller = string.IsNullOrWhiteSpace(Controller) ? null : Controller.Trim(),
            Action = string.IsNullOrWhiteSpace(Action) ? null : Action.Trim(),
            IsAction = IsAction,
            IsActive = IsActive,
            SettingsTabId = string.IsNullOrWhiteSpace(SettingsTabId) ? null : SettingsTabId.Trim(),
            IconClass = string.IsNullOrWhiteSpace(IconClass) ? "bi bi-circle" : IconClass.Trim(),
            Order = Order
        };

        public void ApplyTo(Resource e)
        {
            e.ParentId = ParentId;
            e.Name = Name.Trim();
            e.DeveloperNote = string.IsNullOrWhiteSpace(DeveloperNote) ? null : DeveloperNote.Trim();
            e.Area = Area.Trim();
            e.Controller = string.IsNullOrWhiteSpace(Controller) ? null : Controller.Trim();
            e.Action = string.IsNullOrWhiteSpace(Action) ? null : Action.Trim();
            e.IsAction = IsAction;
            e.IsActive = IsActive;
            e.SettingsTabId = string.IsNullOrWhiteSpace(SettingsTabId) ? null : SettingsTabId.Trim();
            e.IconClass = string.IsNullOrWhiteSpace(IconClass) ? "bi bi-circle" : IconClass.Trim();
            e.Order = Order;
        }
    }
}
