# 🏋️ SportsDiarys

## 📌 Project Overview

**SportsDiarys** is a web application built with ASP.NET Core MVC that allows users to track their training activities, monitor progress, and manage personal fitness goals.

The project demonstrates advanced ASP.NET Core concepts, including layered architecture, dependency injection, Entity Framework Core, Identity authentication, and unit testing.

---

## 🚀 Features

### 👤 User Features

* User registration and login (ASP.NET Identity)
* Personal user profile
* Create and manage training diaries
* Add training entries (duration, calories, distance, exercises)
* Track nutrition targets (calories and protein)
* View progress statistics and summaries
* TDEE calculator

### 🛠️ Admin Features (Admin Area)

* Manage users
* Manage exercises (CRUD)
* Administrative dashboard

### 🔍 Additional Functionality

* Pagination for large datasets
* Search and filtering
* Seeded initial data
* Custom error pages (404 / 500)
* Responsive UI (Bootstrap)

---

## 🧱 Architecture

The project follows a **layered architecture**:

```
SportsDiarys (Web)
│
├── SportsDiarys.Services (Business Logic)
├── SportsDiarys.Data (DbContext, EF Core)
├── SportsDiarys.Data.Models (Entities)
├── SportsDiarys.ViewModels (View Models)
├── SportsDiarys.Common (Constants, Roles)
├── SportsDiarys.Tests (Unit Tests)
```

### Key Principles

* Separation of concerns
* Dependency Injection
* Thin controllers, business logic in services
* Strong cohesion and loose coupling

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
  * CSRF

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

👉 **Coverage: ~90% of services layer**

Tested services include:

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

3. Update connection string in:

```
appsettings.json
```

4. Apply migrations:

```
Update-Database
```

5. Run the project:

```
F5
```

---

## 🔑 Demo Accounts

### Admin

* Email: [admin@sportsdiarys.com](mailto:admin@sportsdiarys.com)
* Password: Admin123!

### User

* Register a new account

---

## 🌐 Deployment

(Optional)

The application can be deployed to:

* Azure App Service
* Azure SQL Database

---

## 📸 Screenshots

*(Optional – add screenshots here)*

---

## 🎥 Demo Video

*(Optional – add link to video presentation)*

---

## 📊 GitHub Repository

* Public repository with full history
* 30+ commits across multiple days
* Logical commit structure

👉 https://github.com/PetqEva/SportsDiarys

---

## ✅ Requirements Coverage

This project fulfills all ASP.NET Advanced course requirements:

* ✔ 10+ views
* ✔ 5+ controllers
* ✔ 5+ entity models
* ✔ MVC + Razor
* ✔ EF Core + SQL Server
* ✔ Identity + roles
* ✔ Admin Area
* ✔ Pagination + search
* ✔ Unit testing (65%+ coverage)
* ✔ Error handling (404 / 500)
* ✔ Validation and security
* ✔ Clean architecture
* ✔ GitHub with proper history
* ✔ Full documentation

---

## ⭐ Conclusion

SportsDiarys demonstrates a complete, well-structured ASP.NET Core MVC application with real-world functionality, strong architecture, and high test coverage.

---

© 2026 SportsDiarys
