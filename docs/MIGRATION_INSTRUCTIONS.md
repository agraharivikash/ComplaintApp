Migration steps to update your database for the Complaint & Issue Tracking Portal

Prerequisites
- .NET 10 SDK installed
- EF Core tools available (dotnet-ef). Install: `dotnet tool install --global dotnet-ef` if needed
- Connection string in `appsettings.json` pointing to your SQL Server instance

Quick steps
1. Update the DbContext: ensure `AppDbContext` exposes `DbSet<Complaint>` and `DbSet<Comment>` (already applied in project).
2. Create a migration locally:
   - Open a terminal in the project folder containing the .csproj and run:
     `dotnet ef migrations add AddComplaintsAndComments`
   - This will create a migration under `Migrations/`.
3. Apply the migration to the database:
   - `dotnet ef database update`
   - This will create the `Complaints` and `Comments` tables and their relations.

If you are using an existing database with data
- The migration added here assumes new tables only; existing tables (People, Users) are unaffected.
- If you use a shared production database, backup before running migrations.
- If you need to transform existing data, edit the generated migration `Up` method to include `Sql("...")` statements to migrate data.

Manual SQL (if you cannot run EF tools)
- Run the following SQL on your database server:

CREATE TABLE Complaints (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Title NVARCHAR(200) NOT NULL,
  Description NVARCHAR(MAX) NULL,
  Category INT NOT NULL,
  Priority INT NOT NULL,
  Status INT NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  UserId INT NULL
);

CREATE TABLE Comments (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Text NVARCHAR(MAX) NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  ComplaintId INT NOT NULL,
  UserId INT NULL,
  CONSTRAINT FK_Comments_Complaints FOREIGN KEY (ComplaintId) REFERENCES Complaints(Id) ON DELETE CASCADE
);

After manual SQL
- Optionally create an empty migration to sync EF Core's snapshot: `dotnet ef migrations add BaselineAfterManualSql --ignore-changes`.

Notes
- The project uses integrated security in the connection string; make sure your SQL Server accepts Windows auth or change to SQL auth.
- For production, prefer to use migrations via `dotnet ef database update` to keep EF ModelSnapshot in sync.

Update-Database -Context AppDbContext
