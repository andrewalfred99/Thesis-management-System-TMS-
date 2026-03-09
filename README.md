# TSU Thesis Management System (TMS)

`TMS` is a Razor Pages web application intended as an official workflow for **Tomsk State University (TSU)** (Tomsk, Russia) to support communication and coordination between **students** and **professors/supervisors** during the student thesis lifecycle.

The system centralizes thesis-related data (topics, details, status/ownership) and enables common academic interactions (requests, notifications, grading) in a single place.

## Key features

- **Authentication & Authorization** using ASP.NET Core Identity
- **Role-based access** for different user types (e.g., Teacher, Student)
- **Thesis management** (create/edit/view/own thesis records)
- **Faculty management** (create/edit/view faculties)
- **Requests & Notifications** between users
- **Grading workflow** for a thesis/related evaluation

> Note: Feature names reflect the current pages in `TMS/Pages/*`.

## Built with

- **ASP.NET Core Razor Pages**
- **Entity Framework Core** (migrations)
- **SQL Server** (via connection string configuration)

## Getting started

### Prerequisites

- .NET SDK (project targets modern .NET; use the SDK specified by the solution/workspace)
- SQL Server instance (LocalDB or full SQL Server)
- EF Core tooling (if you plan to run migrations from CLI)

### Configure the database

This Razor Pages app uses Entity Framework Core migrations.

Update `TMS/appsettings.json` (or `TMS/appsettings.Development.json`) with your SQL Server connection string.

Example (SQL Server):

- `DefaultConnection`: `Server=YOUR_SERVER;Database=TMS;User Id=sa;Password=YOUR_PASSWORD;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`

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

### Run the application

```powershell
dotnet run --project TMS/TMS.csproj
```

Then open the shown URL (typically `https://localhost:xxxx`).

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

## Project structure (high level)

- `TMS/Pages/` — Razor Pages UI (theses, faculties, grades, requests, notifications, users)
- `TMS/Areas/Identity/` — Identity UI and data models
- `TMS/Data/` — domain models and helpers
- `TMS/Models/SeedData.cs` — demo user seeding

## Security & deployment notes

- Do not commit real passwords, live connection strings, or secrets to source control.
- For production, use secure secret storage (environment variables, user-secrets, or a vault) and enforce proper TLS/cert configuration.

## License

If you plan to publish or deploy this for real TSU usage, ensure licensing, ownership, and policy compliance are clearly defined.
