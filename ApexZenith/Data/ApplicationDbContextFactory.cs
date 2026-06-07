using ApexZenith.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApexZenith.Data;

/// <summary>
/// Used only by EF Core tooling (Add-Migration, Update-Database, migrations script).
/// Always targets SQL Server / LocalDB so that migration files in source control
/// are always SQL Server-compatible and ready to deploy to site4now.
///
/// Runtime provider selection (Npgsql in dev, SQL Server in prod) happens in Program.cs.
/// Local dev schema is managed by EnsureCreatedAsync (no Postgres migrations needed).
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
             config.GetConnectionString("DefaultConnection")
            ?? "Data Source=SQL1001.site4now.net;Initial Catalog=db_ac8b9c_beldo55;Persist Security Info=False;User ID=db_ac8b9c_beldo55_admin;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Command Timeout=0";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString, o => o.EnableRetryOnFailure())
            .Options;

        return new ApplicationDbContext(options);
    }
}
