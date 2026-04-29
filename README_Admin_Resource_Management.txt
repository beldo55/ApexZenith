APEXZENITH ADMIN RESOURCE MANAGEMENT SETUP

This project stores admin navigation resources in the database.
The feature is centered on these files:
- ApexZenith/Areas/Admin/Controllers/AdminResourceController.cs
- ApexZenith/Areas/Admin/Models/Resource.cs
- ApexZenith/Areas/Admin/Models/ResourceFormModel.cs
- ApexZenith/Data/ApplicationDbContext.cs
- ApexZenith/Data/SeedData.AdminMenu.cs
- ApexZenith/Program.cs

1. What this feature does

The admin area uses a database-driven menu called "Resources".
Each resource can represent:
- a top-level menu item
- a grouped menu section
- a child menu item under a parent

Each row can also be linked to one or more Identity roles through ResourceRoles.

2. Database objects

The setup uses two tables:
- Resources
- ResourceRoles

They are configured in ApplicationDbContext.cs.
Resource has fields like:
- Id
- ParentId
- Name
- DeveloperNote
- Area
- Controller
- Action
- Order

ResourceRole links a resource row to a role name.

3. Route and admin access

The app maps admin routes using:
- {area:exists}/{controller=Admin}/{action=Index}/{id?}

That means admin pages live under the Admin area.

AdminResourceController is protected with:
- [Authorize(Roles = "Admin")]

So only users in the Admin role can manage resources.

4. How the resource menu is seeded

On application startup, Program.cs runs:
- Database migration
- SeedRolesAsync
- SeedAdminNavigationMenuAsync
- InitializeAsync

SeedAdminNavigationMenuAsync creates the default admin menu entries if the Resources table is empty.
It adds:
- Dashboard
- Site data overview
- System settings
- Website content
- Navigation resources

5. How to set it up from a fresh clone

1. Update the connection string in ApexZenith/appsettings.json or ApexZenith/appsettings.Development.json.
2. Make sure PostgreSQL is running.
3. Run the application once so migrations are applied automatically.
4. Confirm the Resources and ResourceRoles tables were created.
5. Log in with an account that has the Admin role.
6. Open the Admin area and go to the resource management page.

6. Where resource management lives

The CRUD logic is in AdminResourceController.cs.

Create:
- Opens a form with Area preset to Admin.
- Saves the Resource row.
- Saves role links in ResourceRoles.

Edit:
- Loads the selected resource.
- Loads its assigned roles.
- Prevents a row from being its own parent.

Index:
- Shows all resources ordered by parent and display order.

7. Parent-child structure

Resources can be nested by ParentId.
Use a parent row for section headers or grouped navigation.
Use child rows for items under that section.

Important rule:
- Do not set ParentId to the same Id as the row itself.

8. Role setup

When you assign roles to a resource:
- existing role links are removed
- selected roles are inserted again

This means the selected list becomes the source of truth for that menu item.

9. Adding a new admin menu item

To add a new item in the UI:
- open Admin resource management
- click Create
- enter the menu name
- set Area to Admin
- set Controller and Action
- choose Parent if needed
- set Order
- select allowed roles

10. Notes for future changes

If you add new navigation behavior later, keep these files aligned:
- Resource.cs for entity fields
- ApplicationDbContext.cs for table mapping
- SeedData.AdminMenu.cs for default entries
- AdminResourceController.cs for CRUD and role sync

If the menu does not show correctly, verify:
- the user is in the Admin role
- the resource has the correct Area/Controller/Action
- the parent relationship is valid
- the database was migrated successfully
