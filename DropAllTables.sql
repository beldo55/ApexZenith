/* ============================================================================
   FULL RESET for the ApexZenith database (shared-hosting friendly).

   Drops every foreign key, then every user table (including
   __EFMigrationsHistory). After this runs, go to Package Manager Console and
   run:  Update-Database   -- EF rebuilds the schema from migration HJK,
   then start the app so SeedData + the bootstrap admin repopulate.

   ############  WARNING  ############
   THIS DELETES ALL DATA: users, roles, news, content, everything.
   There is no undo. Make a backup first if any data matters.
   ##################################

   Use this only if you cannot run Drop-Database (e.g. site4now shared SQL).
   ============================================================================ */

SET NOCOUNT ON;

DECLARE @sql NVARCHAR(MAX) = N'';

/* 1. Drop all foreign key constraints so tables can be dropped in any order. */
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.'
            + QUOTENAME(t.name) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(10)
FROM sys.foreign_keys AS fk
JOIN sys.tables AS t ON fk.parent_object_id = t.object_id;

EXEC sp_executesql @sql;

/* 2. Drop all user tables. */
SET @sql = N'';
SELECT @sql += N'DROP TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.'
            + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.tables
WHERE is_ms_shipped = 0;

EXEC sp_executesql @sql;

PRINT N'All tables dropped. Now run Update-Database in Package Manager Console.';
