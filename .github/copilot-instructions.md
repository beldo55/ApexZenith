# ApexZenith — AI Coding Agent Instructions

## Project
ASP.NET Core 9 (MVC) marketing/CMS site. Public-facing pages plus an `Admin` area
for managing site content. Persistence is EF Core + SQL Server; auth is ASP.NET
Core Identity with role-based authorization.

## Architecture
- **Public site:** `Controllers/` (e.g. `HomeController`) renders the marketing pages.
- **Admin area:** `Areas/Admin/` holds the CMS — content CRUD, user/role management,
  navigation, and account/login flows. All admin controllers are `[Area("Admin")]`
  and `[Authorize]`.
- **Data:** `Data/ApplicationDbContext.cs` (Identity + domain entities).
  Seeding lives in `Data/SeedData*.cs` and runs at startup in `Program.cs`
  (migrations are applied automatically via `MigrateAsync`).
- **Models:** domain entities in `Models/`; admin-specific view/form models in
  `Areas/Admin/Models/`.

## Conventions
- Content CRUD actions accept an optional `returnTo` and resolve it through an
  allow-list (`AllowedReturnActions`) — never redirect to a raw user-supplied action.
- All POST actions use `[ValidateAntiForgeryToken]`.
- Store `DateTime` values as UTC (`EnsureUtc`).
- Database changes go through EF migrations (`dotnet ef migrations add <Name>`).
  Give migrations descriptive names — not placeholders.

## Security notes
- Never commit real secrets. The production connection string belongs in
  user-secrets or environment variables, not `appsettings.json`.
- `DataProtectionKeys/` and `appsettings.Development.json` are git-ignored — keep them out of source control.
