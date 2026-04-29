using ApexZenith.Models;
using ApexZenith.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ApexZenith.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<About> About => Set<About>();
        public DbSet<Address> Address => Set<Address>();
        public DbSet<Client> Client => Set<Client>();
        public DbSet<Team> Team => Set<Team>();
        public DbSet<Services> Services => Set<Services>();
        public DbSet<Contact> Contact => Set<Contact>();
        public DbSet<Testimonial> Testimonial => Set<Testimonial>();
        public DbSet<NewsCategory> NewsCategories => Set<NewsCategory>();
        public DbSet<News> News => Set<News>();
        public DbSet<Resource> Resource => Set<Resource>();
        public DbSet<ResourceRole> ResourceRoles => Set<ResourceRole>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resources");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IconClass).HasDefaultValue("bi bi-circle");
                entity.Property(e => e.IsAction).HasDefaultValue(true);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Ensure we index for hierarchy queries
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => new { e.ParentId, e.Order });

                // Configure self-referencing relationship
                entity.HasOne<Resource>()
                    .WithMany()
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ResourceRole>(entity =>
            {
                entity.ToTable("ResourceRoles");
                entity.HasKey(e => e.Id);

                // Index on ResourceId for faster lookup
                entity.HasIndex(e => e.ResourceId);

                // Configure the link between ResourceRole and Resource
                entity.HasOne<Resource>()
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
