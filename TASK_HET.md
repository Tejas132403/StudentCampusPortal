# 🟢 HET's Task — Attendance Dashboard + REST API
### Branch: `het` | Assigned to: Het | Estimated time: 3-4 hours

---

## ⚠️ BEFORE YOU START — Read This First

**Step 1: Wait for Tejas to push the base project to `main`.** (He will message you when done.)

**Step 2: Clone the repo and checkout YOUR branch:**
```bash
git clone https://github.com/<REPO-URL>/StudentCampusPortal.git
cd StudentCampusPortal
git checkout het
```

**Step 3: Pull latest from main into your branch:**
```bash
git pull origin main
```

**Step 4: Run the project to confirm it works:**
```bash
dotnet run
# Open http://localhost:5000 — you should see the Login page
```

**Step 5: Register a test account and login to confirm the sidebar appears.**

✅ Only then start coding.

---

## 📋 Your Tasks (in order)

### TASK 1: Implement `DashboardController.cs` — Index action

**File:** `Controllers/DashboardController.cs`

Find the `// TODO (HET):` comment in the `Index()` method. Replace the empty list with a real DB query.

**What to do:**
1. Get `studentId` from session (already shown in the stub)
2. Open a DB connection using `_db.GetConnection()`
3. Query: `SELECT * FROM Attendance WHERE StudentId = @studentId`
4. Loop through the reader and create `AttendanceRecord` objects
5. Return the list to the view: `return View(records);`

**Complete implementation:**
```csharp
public IActionResult Index()
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");
    var records = new List<AttendanceRecord>();

    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(
        "SELECT * FROM Attendance WHERE StudentId = @id ORDER BY Subject", conn);
    cmd.Parameters.AddWithValue("@id", studentId);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        records.Add(new AttendanceRecord
        {
            Id              = (int)reader["Id"],
            StudentId       = studentId,
            Subject         = reader["Subject"].ToString()!,
            Type            = reader["Type"].ToString()!,
            TotalClasses    = (int)reader["TotalClasses"],
            AttendedClasses = (int)reader["AttendedClasses"]
        });
    }

    return View(records);
}
```

**Add this using at the top of the file:**
```csharp
using Microsoft.Data.SqlClient;
```

---

### TASK 2: Implement `AttendanceApiController.cs` — REST API

**File:** `Controllers/Api/AttendanceApiController.cs`

This is Lab 10 (REST API). Implement both endpoints.

**TASK 2A — `GetByEnrollment` endpoint:**
```csharp
[HttpGet("{enrollmentNo}")]
public IActionResult GetByEnrollment(string enrollmentNo)
{
    using var conn = _db.GetConnection();

    // Step 1: Get studentId from enrollment number
    using var studentCmd = new SqlCommand(
        "SELECT Id, Name FROM Students WHERE EnrollmentNo = @e", conn);
    studentCmd.Parameters.AddWithValue("@e", enrollmentNo);
    using var studentReader = studentCmd.ExecuteReader();

    if (!studentReader.Read())
        return NotFound(new { message = "Student not found" });

    int studentId   = (int)studentReader["Id"];
    string name     = studentReader["Name"].ToString()!;
    studentReader.Close();

    // Step 2: Get attendance records
    var records = new List<object>();
    using var attCmd = new SqlCommand(
        "SELECT Subject, Type, TotalClasses, AttendedClasses FROM Attendance WHERE StudentId = @id", conn);
    attCmd.Parameters.AddWithValue("@id", studentId);
    using var attReader = attCmd.ExecuteReader();

    int totalAttended = 0, totalClasses = 0;
    while (attReader.Read())
    {
        int attended = (int)attReader["AttendedClasses"];
        int total    = (int)attReader["TotalClasses"];
        totalAttended += attended;
        totalClasses  += total;
        records.Add(new {
            subject         = attReader["Subject"].ToString(),
            type            = attReader["Type"].ToString(),
            totalClasses    = total,
            attendedClasses = attended,
            percentage      = total == 0 ? 0 : Math.Round((double)attended / total * 100, 1)
        });
    }

    double overall = totalClasses == 0 ? 0 : Math.Round((double)totalAttended / totalClasses * 100, 1);

    return Ok(new {
        student        = enrollmentNo,
        name           = name,
        overallPercent = overall,
        subjects       = records
    });
}
```

**TASK 2B — `GetAll` endpoint (optional but good for demo):**
```csharp
[HttpGet("all")]
public IActionResult GetAll()
{
    var result = new List<object>();
    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(@"
        SELECT s.Name, s.EnrollmentNo, a.Subject, a.Type,
               a.TotalClasses, a.AttendedClasses
        FROM Students s
        JOIN Attendance a ON s.Id = a.StudentId
        ORDER BY s.Name, a.Subject", conn);
    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        result.Add(new {
            name            = reader["Name"].ToString(),
            enrollmentNo    = reader["EnrollmentNo"].ToString(),
            subject         = reader["Subject"].ToString(),
            type            = reader["Type"].ToString(),
            totalClasses    = (int)reader["TotalClasses"],
            attendedClasses = (int)reader["AttendedClasses"]
        });
    }
    return Ok(result);
}
```

---

## ✅ How to Test Your Work

1. Run the project: `dotnet run`
2. Register an account and login
3. Open **Dashboard** — you should see attendance bars for .NET, DAA, FM, CCE
4. Test the API in your browser:
   - `http://localhost:5000/api/attendance/92400103122`
   - `http://localhost:5000/api/attendance/all`
5. Confirm the JSON response looks correct

---

## 📤 How to Push Your Code

```bash
# Stage all your changes
git add .

# Commit with your name
git commit -m "feat: implement attendance dashboard and REST API - Het"

# Push to YOUR branch (never push to main directly)
git push origin het
```

---

## 🔁 How to Create a Pull Request

1. Go to GitHub repo in browser
2. Click **"Compare & pull request"** (appears after push)
3. Set: **base = main**, **compare = het**
4. Title: `feat: Attendance Dashboard + REST API - Het`
5. Description: What you built
6. Click **"Create pull request"**
7. Message Tejas to review and merge

---

## 📂 Files You Should Touch

✅ `Controllers/DashboardController.cs` — implement Index()  
✅ `Controllers/Api/AttendanceApiController.cs` — implement both endpoints  
❌ Do NOT touch: AccountController, FilesController, SettingsController, any Views, Models, Layout

---

## 🚨 Common Mistakes to Avoid

- Do NOT push to `main` branch
- Do NOT edit `_Layout.cshtml` or `site.css`
- Do NOT edit any Model files
- Always use `using var conn = _db.GetConnection()` to avoid connection leaks
- If you get "connection string" error, check `appsettings.json` has the right LocalDB path
