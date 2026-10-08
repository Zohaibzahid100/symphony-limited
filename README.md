# Symphony Limited

A web-based institute management system built with **ASP.NET Core MVC (.NET 10)** and **SQL Server**.

The solution contains two web applications that share the same database (`SymphonyLimited`):

| Project | Purpose |
|---|---|
| `AdminDashbord` | Admin panel to manage the institute |
| `WebApplication1` | Public website and student portal |

## Features

### Admin Dashboard (`AdminDashbord`)
- Admin login and profile
- Manage branches, courses and users
- Manage students and enrollments
- Entrance exams and entrance results
- Final exams and final results
- Lab sessions and track assignment rules
- Payments

### Public Website and Student Portal (`WebApplication1`)
- Home, About, Courses, Branches and Contact pages
- Student sign-in and dashboard
- Enrollment, lab session, payment and result views
- Online entrance exam and final exam
- Certificate page

## Tech Stack
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server (SQL Server Express works)
- Bootstrap and Bootstrap Icons

## Getting Started

### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express
- Visual Studio 2022 or newer (recommended)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/Zohaibzahid100/symphony-limited.git
   cd symphony-limited
   ```

2. Update the connection string in both `AdminDashbord/appsettings.json` and `WebApplication1/appsettings.json` with your own SQL Server name:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_PC_NAME\\SQLEXPRESS;Database=SymphonyLimited;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Create the `SymphonyLimited` database in SQL Server and set up its tables (the models are in the `Models` folder of each project).

4. Run a project:
   ```bash
   dotnet run --project AdminDashbord
   dotnet run --project WebApplication1
   ```
   Or open `Symphony limitted.slnx` in Visual Studio and run the project you want.

## Project Structure

```
Symphony limitted/
├── AdminDashbord/      # Admin panel (Controllers, Models, Views, wwwroot)
├── WebApplication1/    # Public site and student portal
└── Symphony limitted.slnx
```

## Author
Zohaib Zahid
