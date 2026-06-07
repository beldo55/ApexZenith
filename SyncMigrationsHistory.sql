/* ============================================================================
   Sync EF Core migrations history with the existing ApexZenith database.

   WHY: The Migrations folder was reset to a single full-schema baseline
   (20260607221827_HJK) plus an empty follow-up (20260607222219_Kuyt).
   The database already contains every table, so running HJK fails with
   "There is already an object named 'About'". This script instead tells EF
   that those migrations are already applied, and tidies up history rows for
   migration files that no longer exist.

   SAFE TO RE-RUN: every statement is guarded / idempotent. It creates no
   tables and drops no data.

   HOW TO RUN: execute against the ApexZenith database using SQL Server
   Management Studio, Azure Data Studio, Visual Studio's SQL Server Object
   Explorer, or your host's web SQL console (site4now / myLittleAdmin).
   Do NOT run "update-database" until after this script succeeds.
   ============================================================================ */

SET NOCOUNT ON;

/* 0. Safety: make sure the history table exists before we touch it. */
IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    RAISERROR(N'__EFMigrationsHistory not found. Are you connected to the right database?', 16, 1);
    RETURN;
END

/* 1. Correct the Team social column name if the rename never ran.
      (The current model expects InstagramUrl.) Idempotent. */
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE [Name] = N'InstergramUrl' AND [Object_ID] = OBJECT_ID(N'[Team]'))
   AND NOT EXISTS (SELECT 1 FROM sys.columns
           WHERE [Name] = N'InstagramUrl' AND [Object_ID] = OBJECT_ID(N'[Team]'))
BEGIN
    EXEC sp_rename N'[Team].[InstergramUrl]', N'InstagramUrl', N'COLUMN';
    PRINT N'Renamed Team.InstergramUrl -> Team.InstagramUrl';
END

/* 2. Remove history rows for migrations whose .cs files were deleted. */
DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] IN (
    N'20260605021223_InitialCreate',
    N'20260606073512_hhy',
    N'20260607000000_RenameTeamInstagramColumn'
);

/* 3. Mark the regenerated migrations as already applied
      (their schema is already present in the database). */
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260607221827_HJK')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607221827_HJK', N'9.0.15');

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260607222219_Kuyt')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607222219_Kuyt', N'9.0.15');

/* 4. Show the resulting history for confirmation. */
SELECT [MigrationId], [ProductVersion]
FROM [__EFMigrationsHistory]
ORDER BY [MigrationId];
