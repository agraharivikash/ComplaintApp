# Complaint & Issue Tracking Portal — Project Documentation

Version: Workspace snapshot (branch)

Table of contents
1. Project overview
2. Tech stack
3. High-level architecture
4. File map
5. Domain model & DB schema
   - Tables and columns
   - Relationships
6. Migrations (present)
7. Authentication & authorization
8. Controllers and views
9. Running and deployment (PMC and SQL)
10. Generated migration SQL
11. Security notes and recommended improvements
12. Next steps

---

## 1. Project overview

This repository contains an ASP.NET (.NET 10) Razor-based Complaint & Issue Tracking Portal. Features implemented in the workspace:

- Register complaint (category, priority)
- Admin dashboard to update status
- Complaint status lifecycle: `Open ? InProgress ? Resolved`
- Comment history on complaints
- Role-based behavior: `User` and `Admin` (basic implementation)

The shared layout and navigation were corrected to avoid Razor TagHelper conflicts (no `<a>` with both `href` and `asp-*`).

## 2. Tech stack

- .NET 10
- ASP.NET Core MVC / Razor Pages
- Entity Framework Core (SQL Server provider)
- Cookie authentication
- Bootstrap 5 and Font Awesome

## 3. High-level architecture

- Presentation: Razor views in `Views/` and shared layout `Views/Shared/_Layout.cshtml`.
- Business/Controllers: `Controllers/` (Account, Complaints, Admin, Person, Home).
- Data access: `Models/AppDbContext.cs` (EF Core DbContext) and migrations in `Migrations/`.
- Models: `Models/` (User, Person, Complaint, Comment, enums, view models).

## 4. File map (key files)

- `Program.cs` — app startup, routing, auth registration, DbContext registration
- `Models/AppDbContext.cs` — registers `DbSet<Person>`, `DbSet<User>`, `DbSet<Complaint>`, `DbSet<Comment>`
- `Models/User.cs`, `Models/Person.cs`, `Models/Complaint.cs`, `Models/Comment.cs`, `Models/Enums.cs`
- `Controllers/AccountController.cs`, `Controllers/ComplaintsController.cs`, `Controllers/AdminController.cs`
- Views: `Views/Complaints/*`, `Views/Admin/Dashboard.cshtml`, `Views/Account/*`, `Views/Shared/_Layout.cshtml`
- `Migrations/` — EF migration classes and model snapshot
- `docs/MIGRATION_INSTRUCTIONS.md` — human migration instructions
- `Migrations/Script_AllMigrations.sql` — generated idempotent SQL script

## 5. Domain model & DB schema

### Tables and columns (current model)

- `Users`
  - `Id` int IDENTITY PRIMARY KEY
  - `UserName` nvarchar(100) NOT NULL
  - `Email` nvarchar(200) NOT NULL
  - `PasswordHash` nvarchar(max) NOT NULL
  - `CreatedAt` datetime2 NOT NULL
  - `Role` nvarchar(max) NOT NULL (default: `User`)

- `People`
  - `Id`, `FirstName` nvarchar(30) NOT NULL, `LastName` nvarchar(30) NOT NULL, `UserId` int NULL (FK -> Users.Id)

- `Complaints`
  - `Id` int IDENTITY PRIMARY KEY
  - `Title` nvarchar(200) NOT NULL
  - `Description` nvarchar(max) NOT NULL
  - `Category` int NOT NULL
  - `Priority` int NOT NULL
  - `Status` int NOT NULL
  - `CreatedAt` datetime2 NOT NULL
  - `UserId` nvarchar(max) NULL (stores `ClaimTypes.NameIdentifier` as string)

- `Comments`
  - `Id`, `Text` nvarchar(max) NOT NULL, `CreatedAt` datetime2 NOT NULL, `ComplaintId` int NOT NULL (FK -> Complaints.Id), `UserId` nvarchar(max) NULL

### Relationships

- `People.UserId` -> `Users.Id` (ON DELETE SET NULL)
- `Comments.ComplaintId` -> `Complaints.Id` (ON DELETE CASCADE)
- `Complaint.UserId` currently stored as string (no FK). Recommended to convert to `int` FK to `Users.Id`.

## 6. Migrations (present in repo)

- `20260121135609_Initial-Create` — initial People table
- `20260121165815_AddUsersTable` — Users table
- `20260121170620_AddUserIdToPerson` — add UserId column and FK
- `20260121172057_MakeUserIdNullable` — adjust UserId nullability
- `20260122064410_AddComplaintsAndComments` — Complaints and Comments tables
- `20260122071035_AddRoleToUsers` — Role column on Users
- `20260122130000_AddRoleToUsers` (if present) — additional role migration
- `Script_AllMigrations.sql` — idempotent deployment script (see section 10)

## 7. Authentication & authorization

- Cookie authentication configured in `Program.cs`.
- Claims added at sign-in: `ClaimTypes.NameIdentifier`, `ClaimTypes.Name`, `ClaimTypes.Email`, `ClaimTypes.Role`.
- Registration and login views include role selection (`User` or `Admin`) — security risk in production (see recommendations).
- Controllers enforce role/ownership checks manually:
  - `ComplaintsController.Create` requires authenticated users.
  - `ComplaintsController.Index` filters for non-admins to only show their own complaints.
  - `ComplaintsController.Details` requires owner or admin.
  - `AdminController` checks `User.IsInRole("Admin")` for dashboard and status updates.

## 8. Controllers and views (responsibilities)

- `AccountController` — register, login, logout; registers role and emits role claim
- `ComplaintsController` — list (with filter), create (authenticated), details (owner/admin), add comment (owner/admin)
- `AdminController` — admin dashboard and `UpdateStatus` (admin only)

Frontend: `Views/Complaints` contains listing, create, details; `Views/Admin/Dashboard` shows admin controls and status form.

## 9. Running and deployment (PMC and SQL)

Recommended deployment flow using Package Manager Console (PMC):

1. Open Visual Studio ? Tools ? NuGet Package Manager ? Package Manager Console
2. Set the default project (project containing `AppDbContext`):
   ```powershell
   Set-DefaultProject MvcDemo25.web
   ```
3. (If model changed) Add migration(s):
   ```powershell
   Add-Migration AddComplaintsAndComments -Context AppDbContext -OutputDir Migrations
   Add-Migration AddRoleToUsers -Context AppDbContext -OutputDir Migrations
   ```
4. Apply migrations to the database:
   ```powershell
   Update-Database -Context AppDbContext
   ```
5. To generate an idempotent SQL script for deployment:
   ```powershell
   Script-Migration -Context AppDbContext -Idempotent -Output .\Migrations\Script_AllMigrations.sql
   ```

Manual SQL (if EF tools are not available): use `docs/MIGRATION_INSTRUCTIONS.md` or `Migrations/Script_AllMigrations.sql` and run via `sqlcmd` or SSMS.

## 10. Generated migration SQL (excerpt)

Idempotent script location: `Migrations/Script_AllMigrations.sql`.

Key excerpts:

```sql
CREATE TABLE [Complaints] (
  [Id] int IDENTITY PRIMARY KEY,
  [Title] nvarchar(200) NOT NULL,
  [Description] nvarchar(max) NOT NULL,
  [Category] int NOT NULL,
  [Priority] int NOT NULL,
  [Status] int NOT NULL,
  [CreatedAt] datetime2 NOT NULL,
  [UserId] nvarchar(max) NULL
);

CREATE TABLE [Comments] (
  [Id] int IDENTITY PRIMARY KEY,
  [Text] nvarchar(max) NOT NULL,
  [CreatedAt] datetime2 NOT NULL,
  [ComplaintId] int NOT NULL,
  [UserId] nvarchar(max) NULL,
  CONSTRAINT FK_Comments_Complaints FOREIGN KEY ([ComplaintId]) REFERENCES [Complaints]([Id]) ON DELETE CASCADE
);

ALTER TABLE [Users] ADD [Role] nvarchar(max) NOT NULL DEFAULT N'';
```

Use the full generated script for deployment; it is idempotent and safe to run on databases with previous migrations applied.

## 11. Security notes and recommended improvements

1. Prevent public registration as `Admin`. Options:
   - Remove `Role` selection from `Register` view and always set `Role = "User"` server-side.
   - Only allow admin creation via a secure, authenticated admin area or migration seed.

2. Convert `Complaint.UserId` from `string` to `int` and add a FK to `Users.Id` for referential integrity.

3. Replace manual SHA-256 hashing with ASP.NET Identity or a secure password hasher (`IPasswordHasher<T>`), or use PBKDF2/Argon2.

4. Apply `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes on controllers instead of manual checks to simplify code.

5. Ensure anti-forgery tokens are used for POST forms (Razor helper `@Html.AntiForgeryToken()` if needed).

6. Seed a single admin account via a migration for initial setup (development only). Do NOT check-in production credentials.

## 12. Next steps (suggested)

- Harden registration so `Admin` cannot be chosen publicly.
- Implement `Complaint.UserId` as `int` FK and migrate data accordingly.
- Implement proper password hashing (ASP.NET Identity) and consider switching to Identity for full account management.
- Add unit/integration tests for auth flows and permission checks.
- Add CI check that runs Razor view compilation and scans for TagHelper misuse.

---

This document is saved as `docs/PROJECT_DOCUMENTATION.md`. To produce a PDF, open this Markdown in your editor and export or use pandoc:

```powershell
# install pandoc if not available
# convert markdown to pdf
pandoc docs/PROJECT_DOCUMENTATION.md -o docs/PROJECT_DOCUMENTATION.pdf
```
