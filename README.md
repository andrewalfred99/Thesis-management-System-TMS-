# TMS (Thesis Management System)

## Database setup

This Razor Pages app uses Entity Framework Core migrations.

### Connection string

Update `TMS/appsettings.json` (or `TMS/appsettings.Development.json`) with your SQL Server connection string.

Example (SQL Server):

- `DefaultConnection`: `Server=ANDREW-PC\\DBWRK;Database=TMS;User Id=sa;Password=andrew;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`

### Create / update the database

From the repo root:

```powershell
# apply migrations

dotnet ef database update --project TMS/TMS.csproj --startup-project TMS/TMS.csproj
```

If you want a clean database (destructive):

```powershell

dotnet ef database drop -f --project TMS/TMS.csproj --startup-project TMS/TMS.csproj

dotnet ef database update --project TMS/TMS.csproj --startup-project TMS/TMS.csproj
```

## Seeded demo accounts

On app start, `SeedData.SetupUsers(...)` seeds demo users (if they do not already exist) and assigns roles.

### Teacher

- Email: `teacher@tms.local`
- Password: `Teacher1234%`
- Role: `Teacher`

### Student

- Email: `student@tms.local`
- Password: `Student1234%`
- Role: `Student`

Notes:
- The seeded student is linked to a `Faculty` (prefers `HITS`, otherwise the first available faculty).
- Existing seed users (e.g. `teacher1@teacher.com`, `Student1@Student.com`, etc.) are left as-is.
