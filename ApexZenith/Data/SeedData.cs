using ApexZenith.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Data;

public static partial class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to seed role '{roleName}'.");
                }
            }
        }
    }


    public static async Task InitializeAsync(ApplicationDbContext context)
    
    
    {
        if (!await context.About.AnyAsync())
        {
            context.About.Add(new About
            {
                Title = "About ApexZenith",
                Content = "ApexZenith designs and builds practical digital solutions for organizations that want clearer operations, stronger customer experiences, and dependable long-term results.",
                PhotoUrl = "/assets/img/Gemini_Generated_Image_m234sm234sm234sm.png"
            });
        }
        if (!await context.Address.AnyAsync())
        {
            context.Address.Add(new Address
            {
                IsHeadOffice = "Yes",
                Location = "Lagos, Nigeria",
                Email = "info@maincompany.com",
                Phone = "+234 800 000 0000",
                GoogleMapUrl = "https://maps.google.com"
            });
        }

        if (!await context.Services.AnyAsync())
        {
            context.Services.AddRange(
                new Services { Name = "Digital Strategy", Contents = "We help teams define priorities, refine processes, and turn business goals into practical execution plans.", PhotoUrl = "/assets/img/team/team-1.jpg" },
                new Services { Name = "Web Development", Contents = "We build modern websites and web applications that are fast, maintainable, and easy to grow over time.", PhotoUrl = "/assets/img/team/team-1.jpg" },
                new Services { Name = "Business Automation", Contents = "We streamline repetitive work with systems that improve accuracy, speed, and visibility across the business.", PhotoUrl = "/assets/img/team/team-1.jpg" }
            );
        }

        if (!await context.Client.AnyAsync())
        {
            context.Client.AddRange(
                new Client { Name = "Atlas Group", Logo = "/assets/img/clients/client-1.png", Website = "https://example.com" },
                new Client { Name = "North Star Logistics", Logo = "/assets/img/clients/client-2.png", Website = "https://example.com" }
            );
        }

        if (!await context.Team.AnyAsync())
        {
            context.Team.AddRange(
                new Team
                {
                    Name = "Amina Yusuf",
                    Position = "Managing Director",
                    PhotoUrl = "/assets/img/team/team-1.jpg",
                    FacebookUrl = "#",
                    InstagramUrl = "#",
                    LinkedInUrl = "#"
                },
                new Team
                {
                    Name = "Daniel Okafor",
                    Position = "Operations Lead",
                    PhotoUrl = "/assets/img/team/team-2.jpg",
                    FacebookUrl = "#",
                    InstagramUrl = "#",
                    LinkedInUrl = "#"
                }
            );
        }

        if (!await context.Testimonial.AnyAsync())
        {
            context.Testimonial.AddRange(
                new Testimonial
                {
                    Name = "Sarah Bello",
                    Position = "Operations Manager, Atlas Group",
                    Content = "ApexZenith brought structure to our internal workflows and gave us a clearer view of day-to-day operations.",
                    PhotoUrl = "assets/img/testimonials/testimonials-5.jpg"

                },
                new Testimonial
                {
                    Name = "James Musa",
                    Position = "Product Owner, North Star Logistics",
                    Content = "The team was professional, responsive, and focused on delivering something our staff could actually use.",
                    PhotoUrl = "assets/img/testimonials/testimonials-6.jpg"

                }
            );
        }

        if (!await context.NewsCategories.AnyAsync())
        {
            context.NewsCategories.AddRange(
                new NewsCategory { Name = "Business Strategy" },
                new NewsCategory { Name = "Technology" },
                new NewsCategory { Name = "Operations" }
            );
        }

        if (!await context.News.AnyAsync())
        {
            context.News.AddRange(
                new News
                {
                    Headline = "ApexZenith Expands Its Digital Operations Toolkit",
                    Content = "The latest updates improve visibility across content, workflows, and reporting so teams can work with more confidence.",
                    PhotoUrl = "/assets/img/blog/blog-1.jpg",
                    Author = "ApexZenith Editorial Team",
                    PostedBy = "Business Strategy",
                    PostedDate = DateTime.UtcNow.AddDays(-2),
                    Date = DateTime.UtcNow.AddDays(-2),
                    NumberOfViews = 128,
                    IsDeleted = false
                },
                new News
                {
                    Headline = "Why Structured Content Improves Business Communication",
                    Content = "Clear, well-organized information helps organizations respond faster and present a more professional public image.",
                    PhotoUrl = "/assets/img/blog/blog-2.jpg",
                    Author = "ApexZenith Editorial Team",
                    PostedBy = "Technology",
                    PostedDate = DateTime.UtcNow.AddDays(-5),
                    Date = DateTime.UtcNow.AddDays(-5),
                    NumberOfViews = 96,
                    IsDeleted = false
                },
                new News
                {
                    Headline = "Small Workflow Improvements That Create Real Efficiency",
                    Content = "Even modest process changes can reduce manual effort, improve accuracy, and make day-to-day operations easier to manage.",
                    PhotoUrl = "/assets/img/blog/blog-3.jpg",
                    Author = "ApexZenith Editorial Team",
                    PostedBy = "Operations",
                    PostedDate = DateTime.UtcNow.AddDays(-8),
                    Date = DateTime.UtcNow.AddDays(-8),
                    NumberOfViews = 74,
                    IsDeleted = false
                }
            );
        }
        await context.SaveChangesAsync();
    }

    //public static async Task EnsureBootstrapAdminAsync(
    //    UserManager<IdentityUser> userManager,
    //    RoleManager<IdentityRole> roleManager,
    //    IConfiguration configuration,
    //    IHostEnvironment environment)
    //{
    //    var adminEmail = configuration["SeedAdmin:Email"];
    //    var adminPassword = configuration["SeedAdmin:Password"];

    //    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
    //    {
    //        return;
    //    }

    //    var adminRole = "Admin";
    //    if (!await roleManager.RoleExistsAsync(adminRole))
    //    {
    //        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
    //        if (!roleResult.Succeeded)
    //        {
    //            throw new InvalidOperationException("Failed to ensure the Admin role exists.");
    //        }
    //    }

    //    var existingUser = await userManager.FindByEmailAsync(adminEmail);
    //    if (existingUser is null)
    //    {
    //        var user = new IdentityUser
    //        {
    //            UserName = adminEmail,
    //            Email = adminEmail,
    //            EmailConfirmed = true
    //        };

    //        var createResult = await userManager.CreateAsync(user, adminPassword);
    //        if (!createResult.Succeeded)
    //        {
    //            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
    //            throw new InvalidOperationException($"Failed to create bootstrap admin user: {errors}");
    //        }

    //        existingUser = user;
    //    }

    //    if (!await userManager.IsInRoleAsync(existingUser, adminRole))
    //    {
    //        var roleAssignResult = await userManager.AddToRoleAsync(existingUser, adminRole);
    //        if (!roleAssignResult.Succeeded)
    //        {
    //            var errors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
    //            throw new InvalidOperationException($"Failed to assign Admin role to bootstrap user: {errors}");
    //        }
    //    }
    //}
}
