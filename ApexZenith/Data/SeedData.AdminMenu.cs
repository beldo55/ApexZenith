using ApexZenith.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ApexZenith.Data;

public static partial class SeedData
{
    /// <summary>Seeds default admin sidebar rows using only properties defined in the Resource model.</summary>
    public static async Task SeedAdminNavigationMenuAsync(ApplicationDbContext context)
    {
        // Seed the navigation tree once on first run.
        if (await context.Resource.AnyAsync())
        {
            return;
        }

        // 1. Define Top Level Items
        var dashboard = new Resource
        {
            Name = "Dashboard",
            Area = "Admin",
            Controller = "Admin",
            Action = "Index",
            Order = 1,
            IconClass = "bi bi-speedometer2",
            IsActive = true,


        };

        var overview = new Resource
        {
            Name = "Site data overview",
            Area = "Admin",
            Controller = "AdminManage",
            Action = "Index",
            Order = 2,
            IsActive = false,
            IconClass = "bi bi-airplane"
        };

        var systemMap = new Resource
        {
            Name = "System map",
            Area = "Admin",
            Controller = "AdminManage",
            Action = "SystemSettings",
            IsActive = false,
            Order = 3,
            IconClass = "bi bi-diagram-3"
        };

        var system = new Resource
        {
            Name = "System settings",
            Area = "Admin",
            IsActive = true,
            Order = 3,
            IconClass = "bi bi-gear"
        };
        var AccoutManagement = new Resource
        {
            Name = "Accout management",
            Area = "Admin",
            IsActive = true,
            Order = 3,
            IconClass = "bi bi-person-fill-gear"
        };



        // Add and save to generate IDs
        context.Resource.AddRange(dashboard, overview, system, systemMap, AccoutManagement);
        await context.SaveChangesAsync();

        // 2. Define Children (Linked via ParentId since Resource has no Children collection)
        var websiteContent = new Resource
        {
            Name = "Website content",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "Index",
            Order = 1,
            IsActive = false,
            IconClass = "bi bi-eye"

        };

        var navResources = new Resource
        {
            ParentId = AccoutManagement.Id,
            Name = "Resources management",
            Area = "Admin",
            Controller = "AdminResource",
            Action = "Index",
            Order = 1,
            IsActive = true,
        };
        var users = new Resource
        {
            ParentId = AccoutManagement.Id,
            Name = "Users management",
            Area = "Admin",
            Controller = "UsersManagement",
            Action = "Index",
            Order = 2,
            IsAction = true,
            IsActive = true,
            //IconClass = "bi bi-people"
        };
        var roles = new Resource
        {
            ParentId = AccoutManagement.Id,
            Name = "Roles management",
            Area = "Admin",
            Controller = "RolesManagement",
            Action = "Index",
            Order = 3,
            IsAction = true,
            IsActive = true,
            //IconClass = "bi bi-shield-lock"
        };
        var roleAssignments = new Resource
        {
            ParentId = AccoutManagement.Id,
            Name = "Role assignments",
            Area = "Admin",
            Controller = "RoleAssignments",
            Action = "Index",
            Order = 4,
            IsAction = true,
            IsActive = true,
            //IconClass = "bi bi-diagram-3"
        };
        var About = new Resource
        {
            ParentId = system.Id,
            Name = "About",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "About",
            IsAction = true,
            IsActive = true,

            Order = 5
        };
        var News = new Resource
        {
            ParentId = system.Id,
            Name = "News",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "News",
            IsAction = true,
            IsActive = true,
            Order = 5,

        };

        var Services = new Resource
        {
            ParentId = system.Id,
            Name = "Services",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "Services",
            IsAction = true,
            IsActive = true,
            Order = 5,

        };

        var Clients = new Resource
        {
            ParentId = system.Id,
            Name = "Clients",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "Clients",
            IsAction = true,
            IsActive = true,
            Order = 5,

        };

        var Team = new Resource
        {
            ParentId = system.Id,
            Name = "Team",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "TeamMembers",
            IsAction = true,
            IsActive = true,
            Order = 5,

        };

        var Testimonial = new Resource
        {
            ParentId = system.Id,
            Name = "Testimonial",
            Area = "Admin",
            Controller = "AdminContent",
            Action = "Testimonials",
            IsAction = true,
            IsActive = true,
            Order = 5,

        };

        //var Contact = new Resource
        //{
        //    ParentId = system.Id,
        //    Name = "Contact",
        //    Area = "Admin",
        //    Controller = "AdminContent",
        //    Action = "Contact",
        //    IsAction = true,
        //    IsActive = true,
        //    Order = 5,

        //};
        context.Resource.AddRange(websiteContent, navResources, News,Team, Testimonial, Clients, Services, About, users, roles, roleAssignments);
        await context.SaveChangesAsync();

        // 3. Define Roles (Linked via ResourceId since Resource has no RoleRules collection)
        // Using 'RoleId' as defined in your ResourceRole model
        context.ResourceRoles.AddRange(
            new ResourceRole { ResourceId = overview.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = overview.Id, RoleId = "User" },
            new ResourceRole { ResourceId = system.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = websiteContent.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = navResources.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = users.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = roles.Id, RoleId = "Admin" },
            new ResourceRole { ResourceId = roleAssignments.Id, RoleId = "Admin" }
        );

        await context.SaveChangesAsync();
    }
}
