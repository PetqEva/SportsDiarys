# SportsDiarys

SportsDiarys is an ASP.NET Core MVC web application for tracking sports activity, training diaries, training entries, exercises, user profiles, calorie targets, and nutrition goals. The project is designed as an individual final project for the ASP.NET Advanced course and demonstrates layered architecture, role-based authorization, Entity Framework Core, validation, seeding, unit testing, and deployment.

---

## 1. Project Overview

SportsDiarys helps users organize and monitor their sports activity in a structured way. Registered users can create their own profile, maintain training diaries, add training entries, track exercises, calculate calorie needs, and manage nutrition targets. The application also includes an Administration Area for managing users and exercises.

The main goal of the project is to provide a clean, secure, and practical sports diary system built with modern ASP.NET Core development practices.

---

## 2. Main Features

### Public / General Features
- Home page with dashboard overview
- Custom error pages
- Responsive user interface
- Authentication and authorization with ASP.NET Core Identity

### User Features
- Register and log in
- Create and edit personal user profile
- Create, edit, view, and delete training diaries
- Create, edit, view, and delete training entries
- Attach exercises to training entries
- View diary details and entry details
- Search and filter records
- Pagination in list pages
- Calculate calories and nutrition values
- Save personal nutrition targets

### Administrator Features
- Administration Area
- View system statistics
- View all users
- Promote/demote users to administrator
- Manage exercises
- Seeded administrator account and roles

---

## 3. Application Architecture

The solution follows a layered architecture with separation of concerns.

### Projects / Layers
- **SportsDiarys** – ASP.NET Core MVC web application (UI layer)
- **SportsDiarys.Data** – database context, EF Core configuration, and persistence
- **SportDiary.Data.Models** – entity models
- **SportsDiarys.Services** – business logic and service layer
- **SportsDiarys.ViewModels** – view models used by the UI
- **SportsDiarys.GCommon / Common** – shared constants, roles, validation rules, and common helpers
- **SportsDiarys.Tests** – unit tests

### Architecture Principles
- Thin controllers
- Business logic placed in services
- Separation between entity models and view models
- Dependency Injection used throughout the application
- Clear responsibility boundaries between layers
- Role-based access control for administrative functionality

---

## 4. Technologies Used

- **C#**
- **ASP.NET Core MVC**
- **.NET 8**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **ASP.NET Core Identity**
- **Razor Views**
- **Bootstrap**
- **xUnit**
- **FluentAssertions**
- **EF Core InMemory Provider** for tests

---

## 5. Database Models

The application uses multiple entity models. The main ones are:

- **ApplicationUser** – identity user for authentication
- **UserProfile** – user profile data and personal information
- **TrainingDiary** – training diary created by a user
- **TrainingEntry** – entry inside a diary
- **Exercise** – exercise definition
- **TrainingEntryExercise** – link table between entries and exercises
- **NutritionTarget** – personal nutrition targets

This covers the requirement for multiple entity models and demonstrates relational database design with EF Core.

---

## 6. Main Controllers

The application includes multiple controllers, including:

- **HomeController**
- **UserProfilesController**
- **TrainingDiariesController**
- **TrainingEntriesController**
- **CalculatorsController**
- **NutritionTargetsController**

### Admin Area Controllers
- **AdminController**
- **ExercisesController** (inside Admin Area)

This covers the requirement for multiple controllers and MVC Areas.

---

## 7. Identity, Roles, and Authorization

The application uses the built-in ASP.NET Core Identity system.

### Roles
- **User**
- **Administrator**

### Authorization
- Authenticated users can manage only their own profiles, diaries, entries, and targets.
- Administrative functionality is protected with role-based authorization.
- The Admin Area is accessible only to users in the **Administrator** role.

### Security Rules
- Ownership checks are applied when editing, viewing, or deleting personal data.
- Unauthorized users cannot access admin-only pages.
- User-specific data is isolated by profile and identity checks.

---

## 8. Service Layer

The application uses a dedicated service layer to encapsulate business logic.

### Main Services
- **ITrainingDiaryService / TrainingDiaryService**
- **ITrainingEntryService / TrainingEntryService**
- **IExerciseService / ExerciseService**
- **IUserProfileService / UserProfileService**
- **IHomeDashboardService / HomeDashboardService**
- **INutritionTargetService / NutritionTargetService** *(if implemented in the current version)*

### Service Responsibilities
- Data access orchestration
- Business rules
- Filtering and pagination logic
- Ownership validation
- Projection to view models
- CRUD operations
- Error-safe processing

---

## 9. User Interface

The application uses Razor views and Bootstrap-based responsive design.

### UI Features
- Responsive layout
- Navigation for authenticated and anonymous users
- Separate Administration Area
- Search and filter UI
- Pagination controls
- Validation messages
- Confirmation pages for delete operations
- Error pages for invalid requests and server errors

The interface is designed to be clear and easy to use both on desktop and smaller screens.

---

## 10. Validation and Security

Validation and security are important parts of the project.

### Validation
- Required fields
- String length validation
- Numeric range validation
- Server-side validation
- Client-side validation through Razor and validation scripts
- ViewModel-based validation
- Validation constants for reusable rules

### Security
- ASP.NET Core Identity for authentication
- Role-based authorization
- Anti-forgery token protection for POST requests
- Ownership checks for user data
- Protection against SQL Injection through EF Core
- Protection against XSS through Razor HTML encoding
- Prevention of parameter tampering through server-side checks

---

## 11. Pagination, Search, and Filtering

The project includes list pages with pagination and search/filter functionality where appropriate.

### Implemented in modules such as:
- Training diaries listing
- Training entries listing
- Exercise administration pages

This improves usability and satisfies the requirement for pagination and search/filter support.

---

## 12. Error Handling

The application includes custom error handling pages.

### Implemented pages
- **404 Not Found**
- **500 Internal Server Error / application error page**

Additional error handling is implemented to prevent crashes when invalid input or invalid routes are used.

---

## 13. Seeding

The application includes data seeding for initial setup.

### Seeded Data
- Roles: **User** and **Administrator**
- Administrator account
- Initial exercises
- Sample application data where applicable

### Seeder Components
- **IdentitySeeder**
- **DbSeeder**

Seeding helps the project start with usable initial data and demonstrates practical setup of roles and administration.

---

## 14. Project Structure

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SportsDiarysDb;Trusted_Connection=True;"
}