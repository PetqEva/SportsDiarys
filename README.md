# SportsDiarys

SportsDiarys is an ASP.NET Core MVC web application for tracking personal training sessions, exercises, and fitness activity history.

The project demonstrates layered architecture, ASP.NET Core Identity, role-based authorization, CRUD operations, validation, and clean MVC design.

---

## 🚀 Features

- User registration and login (ASP.NET Core Identity)
- Role-based authorization (User / Administrator)
- Personal user profiles
- Training diary management
- Training entries management
- Exercises management (Admin area)
- Search and filtering functionality
- Pagination for lists
- Server-side and client-side validation
- Responsive UI with Bootstrap

---

## 🏗 Architecture

The solution follows a layered architecture:

- **SportsDiarys (Web)** – Controllers, Views, UI
- **SportsDiarys.Data** – DbContext and Entity models
- **SportsDiarys.Services** – Business logic layer
- **SportsDiarys.ViewModels** – ViewModels
- **SportsDiarys.GCommon** – Validation and shared constants

### Principles:
- Separation of concerns
- Dependency Injection
- Thin controllers
- Service-based logic

---

## 🛠 Technologies Used

- ASP.NET Core (.NET 8)
- MVC Architecture
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- Bootstrap 5
- xUnit (for testing)
- Git & GitHub

---

## 🔐 Authentication & Authorization

The application uses ASP.NET Core Identity.

- Registered users can manage only their own data
- Admin users have access to an Admin area
- Role-based access control is implemented

---

## 🧑‍💼 Admin Area

The application includes a dedicated Admin Area:

- Manage users (promote/demote roles)
- Manage exercises (CRUD + activate/deactivate)
- Dashboard with statistics

---

## 🔍 Search, Filtering & Pagination

- Search in exercises and diaries
- Filtering in training entries
- Pagination implemented across multiple pages
- Page size control supported

---

## 🗄 Database Setup

1. Configure connection string in:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SportsDiarysDb;Trusted_Connection=True;"
}