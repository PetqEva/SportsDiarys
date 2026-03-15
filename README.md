# SportsDiarys

SportsDiarys is an ASP.NET Core MVC web application for tracking personal training sessions and managing fitness activity history.

The project demonstrates **layered architecture**, ASP.NET Core Identity authentication, CRUD operations, validation, and clean MVC structure.

---

## 🚀 Features

- User registration and login (ASP.NET Core Identity)
- Personal user profiles
- Training diary management
- Training entries (Create, Edit, Delete, Details)
- "Only my data" security filtering
- Server-side and client-side validation
- Responsive UI with Bootstrap
- Clean layered architecture (Data / Services / Web)

---

## 🏗 Architecture

The solution follows a layered structure:

- **SportsDiarys (Web)** – Controllers, Views, UI
- **SportsDiarys.Data** – DbContext and Entity models
- **SportsDiarys.Services** – Business logic (Service layer)
- **SportsDiarys.ViewModels** – ViewModels used in forms and views
- **SportsDiarys.GCommon** – Validation constants and shared utilities

Controllers do not access the database directly.  
All business logic is handled through services using Dependency Injection.

---

## 🛠 Technologies Used

- ASP.NET Core (.NET 8)
- MVC Architecture
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- Bootstrap 5
- Git & GitHub

---

## 🔐 Authentication & Authorization

The application uses ASP.NET Core Identity.

- Users must register and log in
- Each user can access only their own training data
- Protected pages require authentication

### Demo user (seed)

On local run, the app seeds a demo account + sample diary/entries (see `Infrastructure/DbSeeder.cs`):

- Email: `demo@sportdiary.local`
- Password: `demo123`

If you do not want seed data for submission, comment out the seeding line in `Program.cs`.

---

## 🗄 Database Setup

1. Configure SQL Server connection string in:

   - `SportsDiarys/appsettings.json` or `SportsDiarys/appsettings.Development.json`
   - Key: `ConnectionStrings:DefaultConnection`

   Example:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SportsDiarysDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }

   Apply migrations:

Option A (Package Manager Console):

Update-Database -Project SportsDiarys.Data -StartupProject SportsDiarys

Option B (CLI):

dotnet ef database update --project SportsDiarys.Data --startup-project SportsDiarys

Run the application.

▶ How to Run the Project

Clone the repository.

Open SportsDiarys.sln.

Restore NuGet packages.

Apply migrations (see Database Setup).

Run SportsDiarys (the Web project).

The application will start using the default configuration.

⚡ CRUD Operations

Training Diary – Create, Edit, Delete, View

Training Entries – Create, Edit, Delete, View

Exercises – Add to entries, View details

📂 GitHub Setup

The project is maintained in a public GitHub repository: PetqEva/SportsDiarys

Minimum 10 commits with activity across at least 3 different days

Commit messages describe specific changes (e.g., added validation, fixed views, updated services)

📌 Project Purpose

This project was developed as part of the ASP.NET Fundamentals course assignment.

It demonstrates understanding of:

MVC pattern

Identity integration

Dependency Injection

Layered architecture

CRUD operations

Data validation

Secure user-based data access

🔧 Environment / Credentials

Demo account for testing:

Email: demo@sportdiary.local

Password: demo123

To change the connection string or seed data, update appsettings.json or comment out seeding in Program.cs.

👩‍💻 Author

PetqEva