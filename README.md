# 🏋️ SportsDiarys

## 📌 Project Overview

**SportsDiarys** is a full-stack ASP.NET Core MVC web application designed for tracking training activities, monitoring fitness progress, and managing personal health goals.

The project demonstrates advanced ASP.NET Core concepts including layered architecture, dependency injection, Entity Framework Core, Identity-based authentication, role management, and unit testing.

---

## 🚀 Features

### 👤 User Features

### 🏠 Home Dashboard
![Home](screenshots/home.png)

### 🛠️ Admin Panel (Exercises Management)
![Admin](screenshots/admin.png)

### 🔥 TDEE Calculator
![TDEE](screenshots/tdee.png)

### 📊 Progress & Statistics
![Progress](screenshots/progress.png)

### ⚠️ Error Handling (404 Page)
![404](screenshots/404.png)

### 👤 User Features

* User registration and authentication (ASP.NET Core Identity)
* Personal user profile
* Create, edit, and delete training diaries
* Add training entries (duration, calories, distance)
* Attach exercises to training entries (sets, reps, weight)
* Track nutrition targets (calories and protein)
* View progress statistics and summaries
* TDEE (Total Daily Energy Expenditure) calculator

---

### 🛠️ Admin Features (Admin Area)

* Administrative dashboard
* Manage users
* Manage exercises (CRUD operations)
* Role-based access control

---

### 🔍 Additional Functionality

* Pagination for large datasets
* Search and filtering capabilities
* Seeded initial data
* Custom error pages (404, 403, 500)
* Responsive UI (Bootstrap)
* Clean and user-friendly interface

---

## 🧱 Architecture

The project follows a **layered architecture**:

```
SportsDiarys (Web)
│
├── SportsDiarys.Services        (Business Logic)
├── SportsDiarys.Data            (DbContext, EF Core)
├── SportsDiarys.Data.Models     (Entities)
├── SportsDiarys.ViewModels      (View Models)
├── SportsDiarys.Common          (Constants, Roles)
├── SportsDiarys.Tests           (Unit Tests)
```

### Key Principles

* Separation of concerns
* Dependency Injection
* Thin controllers (logic moved to services)
* Strong cohesion and loose coupling
* Clean and maintainable code

---

## 🧰 Technologies Used

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core
* Microsoft SQL Server
* ASP.NET Core Identity
* Razor Views
* Bootstrap
* xUnit
* FluentAssertions
* InMemory Database (for testing)

---

## 🗄️ Database Models

Main entities:

* UserProfile
* TrainingDiary
* TrainingEntry
* Exercise
* TrainingEntryExercise (many-to-many)
* NutritionTarget

---

## 🔐 Security and Validation

* ASP.NET Identity for authentication and authorization
* Role-based access (User / Administrator)
* Anti-forgery protection (`[ValidateAntiForgeryToken]`)
* Input validation via Data Annotations
* Protection against:

  * SQL Injection (via EF Core)
  * XSS (escaped output)
  * CSRF (anti-forgery tokens)

---

## 🌱 Data Seeding

The application seeds:

* Default roles (User, Administrator)
* Admin account
* Sample exercises
* Demo data for easier testing

---

## 🧪 Unit Tests

* Implemented using **xUnit + FluentAssertions**
* InMemory database for isolation
* Covers core business logic (services layer)

**Coverage:** ~90% of services layer

Tested services:

* TrainingDiaryService
* TrainingEntryService
* ExerciseService
* NutritionTargetService
* ProgressService
* UserProfileService
* HomeDashboardService

---

## ⚙️ How to Run Locally

1. Clone the repository:

```
git clone https://github.com/PetqEva/SportsDiarys
```

2. Open the solution in Visual Studio 2022

3. Configure your database connection using **User Secrets** or environment variables

Example connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SportsDiarysDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

4. Apply migrations:

```
Update-Database
```

5. Run the project:

```
F5
```

> ⚠️ Sensitive credentials are NOT stored in the repository.
> Use local secrets or environment variables for database configuration.

---

## 🔑 Demo Accounts

### Admin

* Email: [admin@sportdiary.bg](mailto:admin@sportdiary.bg)
* Password: Admin123!

### User

* Register a new account through the application

---

## 🌐 Deployment

The application can be deployed to:

* Azure App Service
* Azure SQL Database

Typical deployment steps:

1. Publish the project to Azure App Service
2. Configure connection string in Azure App Settings
3. Run migrations on the production database

---

## 📊 GitHub Repository

* Public repository with consistent development history
* 40+ commits across multiple days
* Clear and descriptive commit messages

👉 https://github.com/PetqEva/SportsDiarys

---

## ✅ Requirements Coverage

This project fulfills all ASP.NET Advanced course requirements:

* ✔ ASP.NET Core MVC application (.NET 6+)
* ✔ 10+ views
* ✔ 5+ controllers
* ✔ 5+ entity models
* ✔ MVC + Razor
* ✔ Entity Framework Core + SQL Server
* ✔ ASP.NET Identity + roles
* ✔ Admin Area
* ✔ Pagination, search, and filtering
* ✔ Unit testing (65%+ coverage)
* ✔ Error handling (404 / 500)
* ✔ Security and validation
* ✔ Layered architecture and SOLID principles
* ✔ GitHub repository with proper history
* ✔ Full project documentation

---

## ⭐ Conclusion

SportsDiarys is a complete, production-ready ASP.NET Core MVC application that demonstrates real-world functionality, clean architecture, strong separation of concerns, and high test coverage.

The project follows best practices for modern web development and meets all requirements for the ASP.NET Advanced course.

---

© 2026 SportsDiarys
