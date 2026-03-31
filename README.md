# 🏋️ SportsDiarys

**SportsDiarys** is a full-featured ASP.NET Core MVC web application for tracking sports activity, training diaries, exercises, and personal fitness progress.

The project is developed as a final diploma project and demonstrates modern software engineering practices including layered architecture, clean code, unit testing, and role-based authorization.

---

# 📌 Project Overview

SportsDiarys allows users to organize and monitor their training routines in a structured and intuitive way.

Each user can:
- Create and manage personal training diaries
- Add training entries with detailed metrics
- Attach exercises to each training entry
- Track calories, duration, distance, and hydration
- Analyze activity through a dashboard

The system also includes an **Administration Area** for managing exercises and system data.

---

# 🧱 Architecture

The application follows a **layered architecture**:
SportsDiarys
│
├── SportsDiarys (Web Layer)
├── SportsDiarys.Services (Business Logic)
├── SportsDiarys.Data (Data Access)
├── SportsDiarys.ViewModels (UI Models)
└── SportsDiarys.Tests (Unit Tests)

### Key Principles:
- Separation of concerns
- Dependency Injection
- Clean architecture
- Testability

---

# ⚙️ Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap 5
- xUnit
- FluentAssertions
- InMemory Database (for testing)

---

# 🔐 Authentication & Authorization

- ASP.NET Core Identity is used for user management
- Role-based authorization:
  - **User**
  - **Administrator**

Admin users have access to:
- Exercise management
- Admin dashboard

---

# 🚀 Main Features

## 👤 User Features
- Registration and login
- User profile creation
- Personal dashboard

## 📓 Training Diaries
- Create diary per day
- Track:
  - Duration
  - Calories
  - Water intake
  - Distance
  - Notes

## 📊 Training Entries
- Add multiple entries per diary
- Filter and search entries
- Pagination support
- Sorting (date, calories, duration, distance)

## 🏋️ Exercises
- Add exercises to entries
- Track:
  - Sets
  - Reps
  - Weight
  - Duration

## 📈 Dashboard
- Total diaries
- Total entries
- Total duration
- Water consumption
- Recent activities

## ⚙️ Admin Area
- Manage exercises
- Activate / deactivate exercises
- View system statistics

---

# 🧪 Unit Testing

The project includes **comprehensive unit tests** using:

- xUnit
- FluentAssertions
- EF Core InMemory database

### Coverage:
- Services layer fully tested
- Business logic validation
- CRUD operations
- Edge cases

### Example tested services:
- TrainingEntryService
- ExerciseService
- TrainingDiaryService
- HomeDashboardService

---

# 🗄️ Database

- Code First approach with Entity Framework Core
- Relationships:
  - One-to-Many (Diary → Entries)
  - Many-to-Many (Entries ↔ Exercises)

### Seeding:
- Default roles (User, Administrator)
- Admin user
- Sample exercises

---

# 🖥️ UI / UX

- Responsive design with Bootstrap
- Clean and intuitive interface
- Notifications using TempData
- Validation messages
- Filtering and pagination UI

---

# ▶️ How to Run the Project

1. Clone the repository:
```bash
git clone https://github.com/PetqEva/SportsDiarys

Open in Visual Studio
Apply migrations:

Update-Database

Run the project:

Ctrl + F5

Run Tests

dotnet test

Deployment (Optional)

The project can be deployed to:

Azure App Service
IIS
Docker (optional)
🎯 Project Goals
Build a real-world ASP.NET Core application
Apply layered architecture
Implement clean and maintainable code
Ensure high test coverage
Demonstrate full-stack development skills
📚 Conclusion

SportsDiarys is a complete web application that demonstrates:

Strong architectural design
Clean separation of layers
Robust business logic
High-quality unit testing
Practical real-world functionality

This project reflects a solid understanding of modern ASP.NET Core development and is suitable as a diploma project or professional portfolio entry.

👩‍💻 Author

Petq

📌 License

This project is for educational purposes.

