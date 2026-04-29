using System.ComponentModel.DataAnnotations;

namespace ApexZenith.Areas.Admin.Models;

public class LoginViewModel
{
    [Required, EmailAddress, Display(Name = "Work email")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember this device")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterStaffViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 12), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required, Display(Name = "Role")]
    public string RoleName { get; set; } = "User";
}

public class UserManagementIndexViewModel
{
    public List<UserManagementRowViewModel> Users { get; set; } = new();
    public UserManagementEditViewModel EditModel { get; set; } = new();
}

public class UserManagementRowViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UserManagementEditViewModel
{
    public string? Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Confirmed")]
    public bool EmailConfirmed { get; set; }

    [StringLength(100, MinimumLength = 12)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string? ConfirmPassword { get; set; }
}

public class RoleManagementIndexViewModel
{
    public List<RoleManagementRowViewModel> Roles { get; set; } = new();
}

public class RoleManagementRowViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
}

public class RoleAssignmentIndexViewModel
{
    public List<RoleAssignmentUserRowViewModel> Users { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}

public class RoleAssignmentUserRowViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
