# 🎓 Student Campus Portal — Master Project Guide
### For: AI Agents, Developers, and Team Members

---

## 📌 Project Overview

**Name:** Student Campus Portal  
**Type:** ASP.NET Core MVC Web Application (.NET 8)  
**Subject:** .NET Technologies (01CE1523) — College Submission  
**Team:** Tejas Andodariya, Het, Bhavya  
**College:** Marwadi University | Class: 5EC2 | Batch: A  

**Purpose:** A dashboard-style web portal for students to:
- View subject-wise attendance with progress bars
- Upload and download study files by subject
- Manage their profile and settings

---

## 🗂️ Repository Branch Structure

```
main           ← Protected. Only Tejas merges here. Base project lives here.
├── tejas      ← Tejas's work: Login, Register, Profile, Layout, DB setup
├── het        ← Het's work: Dashboard attendance + REST API
└── bhavya     ← Bhavya's work: File management + Settings
```

**RULE:** Never push directly to `main`. Always work on your own branch and create a Pull Request.

---

## 📁 Complete Project Structure

```
StudentCampusPortal/
├── Controllers/
│   ├── AccountController.cs       ✅ DONE by Tejas — Login, Register, Logout
│   ├── ProfileController.cs       ✅ DONE by Tejas — Profile page
│   ├── DashboardController.cs     🔧 STUB — Het must implement
│   ├── FilesController.cs         🔧 STUB — Bhavya must implement
│   ├── SettingsController.cs      🔧 STUB — Bhavya must implement
│   └── Api/
│       └── AttendanceApiController.cs  🔧 STUB — Het must implement
├── Models/
│   ├── Student.cs                 ✅ Done
│   ├── AttendanceRecord.cs        ✅ Done
│   ├── SubjectFile.cs             ✅ Done
│   ├── LoginViewModel.cs          ✅ Done
│   ├── RegisterViewModel.cs       ✅ Done
│   └── ChangePasswordViewModel.cs ✅ Done
├── Data/
│   └── DatabaseHelper.cs          ✅ Done — ADO.NET helper, DB init, seeding
├── Views/
│   ├── Shared/_Layout.cshtml      ✅ Done — Sidebar layout
│   ├── Account/Login.cshtml       ✅ Done
│   ├── Account/Register.cshtml    ✅ Done
│   ├── Profile/Index.cshtml       ✅ Done
│   ├── Dashboard/Index.cshtml     🔧 STUB — Het fills controller data
│   ├── Files/Index.cshtml         🔧 STUB — Bhavya fills controller data
│   └── Settings/Index.cshtml      🔧 STUB — Bhavya fills controller data
├── wwwroot/
│   ├── css/site.css               ✅ Done — All styles
│   └── uploads/                   ← File storage (git-ignored)
├── Program.cs                     ✅ Done — App startup + DI + Session
├── appsettings.json               ✅ Done — DB connection string
├── AGENTS.md                      ← This file
├── TASK_HET.md                    ← Het's full AI-ready task instructions
└── TASK_BHAVYA.md                 ← Bhavya's full AI-ready task instructions
```

---

## 🗄️ Database (SQL Server LocalDB)

Connection string is in `appsettings.json`. Database and tables are created **automatically on first run** via `DatabaseHelper.InitializeDatabase()`.

### Tables:

```sql
Students (Id, Name, EnrollmentNo, Email, PasswordHash, Class, Batch)
Attendance (Id, StudentId, Subject, Type, TotalClasses, AttendedClasses)
StudentFiles (Id, StudentId, FileName, StoredFileName, Subject, FilePath, UploadedAt, FileSizeBytes)
```

### Key Notes:
- Passwords are hashed with SHA-256 (see `AccountController.HashPassword`)
- Session stores: `StudentId`, `StudentName`, `EnrollmentNo`, `Class`, `Batch`
- Demo attendance data is auto-seeded when a new student registers

---

## 🔐 Auth Flow

1. User visits `/` → redirected to `/Account/Login`
2. Login stores student data in **Session** (30 min timeout)
3. Optional **Cookie** for "Remember Me" (7 days)
4. All pages check `HttpContext.Session.GetString("EnrollmentNo") != null`
5. Logout clears session + deletes cookie

---

## 🚀 How to Run

```bash
# Make sure .NET 8 SDK is installed
cd StudentCampusPortal
dotnet run
# Open: http://localhost:5000
```

**First run:** Register a new account → attendance data is auto-seeded.

---

## 📚 Lab Coverage

| Lab | Concept | Where in Code |
|-----|---------|---------------|
| Lab 4 | ASP.NET Validation | LoginViewModel, RegisterViewModel attributes |
| Lab 5 | Session & Cookies | AccountController Login/Logout |
| Lab 6 | MVC Architecture | All Controllers + Models + Views |
| Lab 7 | Layout + Partial Views | `_Layout.cshtml` sidebar |
| Lab 8 | ADO.NET CRUD | DatabaseHelper + all Controllers |
| Lab 10 | REST API | AttendanceApiController |
| Lab 11 | Integrated Portal | This entire project |

---

## 🤝 Team Workflow

1. **Tejas** sets up repo, pushes base to `main`
2. **Het** and **Bhavya** clone repo, checkout their branches
3. Each person works only on their assigned files (see TASK files)
4. When done, push to branch and create Pull Request to `main`
5. **Tejas** reviews the PR and merges it

---

## ❓ Dependency Order

```
[Tejas] pushes base to main
         ↓ (then Het and Bhavya can start)
[Het]   implements DashboardController + AttendanceApiController
[Bhavya] implements FilesController + SettingsController
         ↓ (both create PRs)
[Tejas] reviews and merges both PRs
         ↓
[All]   final testing together
```

Het and Bhavya can work **in parallel** — there is no dependency between them.
