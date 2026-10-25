# 🔵 TEJAS's Task — Project Lead + Setup + Login + Review
### Branch: `tejas` | Role: Project Lead + Code Reviewer

---

## ✅ What's Already Done (Base Project — push to main FIRST)

The following is already built and working:
- `Controllers/AccountController.cs` — Login, Register, Logout with Session + Cookies
- `Controllers/ProfileController.cs` — Profile page
- `Models/` — All model files
- `Data/DatabaseHelper.cs` — DB connection, table creation, seed data
- `Views/Shared/_Layout.cshtml` — Sidebar layout
- `Views/Account/Login.cshtml` + `Register.cshtml`
- `Views/Profile/Index.cshtml`
- `wwwroot/css/site.css` — All styles
- `Program.cs` — App startup with Session + DI

---

## 📋 Step-by-Step: First Push to GitHub

### Step 1: Create GitHub Repo
1. Go to https://github.com/new
2. Repository name: `StudentCampusPortal`
3. Private (recommended) or Public
4. Do NOT initialize with README (we already have code)
5. Click **Create repository**

### Step 2: Push Base Code to Main
```bash
cd "D:\.NET\.net project\StudentCampusPortal"
git init
git add .
git commit -m "feat: initial project setup - base layout, auth, DB - Tejas"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/StudentCampusPortal.git
git push -u origin main
```

### Step 3: Create Branches
```bash
# Create and push tejas branch
git checkout -b tejas
git push origin tejas

# Create and push het branch
git checkout main
git checkout -b het
git push origin het

# Create and push bhavya branch
git checkout main
git checkout -b bhavya
git push origin bhavya

# Go back to your working branch
git checkout tejas
```

### Step 4: Add Collaborators on GitHub
1. Settings → Collaborators → Add people
2. Add Het's GitHub username
3. Add Bhavya's GitHub username

### Step 5: Message Het and Bhavya
Tell them:
- Repo URL: `https://github.com/YOUR_USERNAME/StudentCampusPortal`
- Read the file `TASK_HET.md` (for Het) or `TASK_BHAVYA.md` (for Bhavya)
- Their branch is ready to checkout

---

## 📋 Your Own Coding Tasks (on `tejas` branch)

These are smaller improvements you make on your own branch:

1. **Add `.gitignore`** — already in project, verify it ignores `bin/`, `obj/`, `wwwroot/uploads/`
2. **Add `README.md`** — brief project description (see template below)
3. **Test the full login flow** — register, login, logout, check session
4. **Seed a test student** in DB for demo (enrollment: 92400103122, password: tejas123)

---

## 🔍 How to Review Pull Requests

When Het or Bhavya creates a PR:

1. Open GitHub → Pull Requests tab
2. Click the PR
3. Click **"Files changed"** tab → review the code
4. Check:
   - Do they only change their assigned files?
   - Does the code compile without errors?
   - Are there any obvious bugs?
5. Leave a comment if something is wrong (request changes)
6. If all good → click **"Merge pull request"**
7. After merging, pull the update locally:
   ```bash
   git checkout main
   git pull origin main
   ```

---

## 📄 README.md Template (add this to the repo)

```markdown
# Student Campus Portal
### .NET Technologies Lab — Team Project | 5EC2 | Marwadi University

A student dashboard web portal built with ASP.NET Core MVC.

## Team
| Name | Enrollment | Role | Branch |
|------|-----------|------|--------|
| Tejas Andodariya | 92400103122 | Lead + Auth + Review | tejas |
| Het | [enrollment] | Attendance Dashboard + API | het |
| Bhavya | [enrollment] | File Management + Settings | bhavya |

## Features
- Login / Register with Session & Cookie auth
- Subject-wise Attendance Dashboard with progress bars
- File Upload / Download / Delete by subject folder
- Profile page
- Settings with password change

## Tech Stack
- ASP.NET Core MVC (.NET 8)
- ADO.NET (SQL Server LocalDB)
- Bootstrap 5 + Bootstrap Icons

## How to Run
1. Install .NET 8 SDK
2. Clone this repo
3. `cd StudentCampusPortal`
4. `dotnet run`
5. Open http://localhost:5000
6. Register a new account to get started

## Lab Coverage
Labs 4, 5, 6, 7, 8, 10, 11 from .NET Technologies (01CE1523)
```

---

## 🚨 After All PRs Merged — Final Checklist

- [ ] Login works
- [ ] Register works (auto-seeds attendance data)
- [ ] Dashboard shows attendance bars
- [ ] Files upload, download, delete work
- [ ] Profile shows correct info
- [ ] Settings change password works
- [ ] API endpoint `/api/attendance/92400103122` returns JSON
- [ ] All 3 branches have commits from each person
- [ ] PR history shows your reviews and merges
