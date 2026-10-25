# 🟢 HET's Task — Attendance Dashboard + REST API + File Management + Settings
### Branch: `het` (own tasks) + `bhavya` (Bhavya's tasks assigned to Het)
### Assigned to: **HET ONLY** | Estimated time: 6-7 hours

---

## ⚠️ BEFORE YOU START — Read This First

**Step 1: Wait for Tejas to confirm the base project is pushed to `main`.**

**Step 2: Clone the repo:**
```bash
git clone https://github.com/Tejas132403/StudentCampusPortal.git
cd StudentCampusPortal
```

**Step 3: Run the project to confirm it works:**
```bash
dotnet run
# Open http://localhost:5000 — you should see the Login page
```

**Step 4: Register a test account and login.**

✅ Only then start coding.

---

## 📋 YOUR BRANCHES

You are responsible for **TWO branches**:
- **`het`** — Attendance Dashboard + REST API
- **`bhavya`** — File Management + Settings

Work on them one at a time. Finish `het` first, then do `bhavya`.

---

# ═══ PART 1: `het` branch ═══════════════════════════════════════

```bash
git checkout het
git pull origin main
```

---

## TASK 1: Implement `DashboardController.cs` — Index action

**File:** `Controllers/DashboardController.cs`

Replace the entire `Index()` method body with:

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

**Add this using at the top of `DashboardController.cs`:**
```csharp
using Microsoft.Data.SqlClient;
```

---

## TASK 2: Implement `AttendanceApiController.cs` — REST API (Lab 10)

**File:** `Controllers/Api/AttendanceApiController.cs`

**Replace `GetByEnrollment` method:**
```csharp
[HttpGet("{enrollmentNo}")]
public IActionResult GetByEnrollment(string enrollmentNo)
{
    using var conn = _db.GetConnection();

    using var studentCmd = new SqlCommand(
        "SELECT Id, Name FROM Students WHERE EnrollmentNo = @e", conn);
    studentCmd.Parameters.AddWithValue("@e", enrollmentNo);
    using var studentReader = studentCmd.ExecuteReader();

    if (!studentReader.Read())
        return NotFound(new { message = "Student not found" });

    int studentId = (int)studentReader["Id"];
    string name   = studentReader["Name"].ToString()!;
    studentReader.Close();

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
    return Ok(new { student = enrollmentNo, name, overallPercent = overall, subjects = records });
}
```

**Replace `GetAll` method:**
```csharp
[HttpGet("all")]
public IActionResult GetAll()
{
    var result = new List<object>();
    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(@"
        SELECT s.Name, s.EnrollmentNo, a.Subject, a.Type, a.TotalClasses, a.AttendedClasses
        FROM Students s JOIN Attendance a ON s.Id = a.StudentId
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

**Add this using at the top of `AttendanceApiController.cs`:**
```csharp
using Microsoft.Data.SqlClient;
```

---

## ✅ Test Part 1

1. `dotnet run`
2. Login → Dashboard shows attendance bars ✅
3. Visit `http://localhost:5000/api/attendance/YOUR_ENROLLMENT` → JSON response ✅

## 📤 Push Part 1

```bash
git add .
git commit -m "feat: implement attendance dashboard and REST API - Het"
git push origin het
```

---

# ═══ PART 2: `bhavya` branch ════════════════════════════════════

```bash
git checkout bhavya
git pull origin main
```

---

## TASK 3: Implement `FilesController.cs` — Index (List Files)

**File:** `Controllers/FilesController.cs`

**Add this using at the top:**
```csharp
using Microsoft.Data.SqlClient;
```

**Replace the `Index()` method body:**
```csharp
public IActionResult Index()
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");
    var files = new List<SubjectFile>();

    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(
        "SELECT * FROM StudentFiles WHERE StudentId = @id ORDER BY Subject, UploadedAt DESC", conn);
    cmd.Parameters.AddWithValue("@id", studentId);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        files.Add(new SubjectFile
        {
            Id             = (int)reader["Id"],
            StudentId      = studentId,
            FileName       = reader["FileName"].ToString()!,
            StoredFileName = reader["StoredFileName"].ToString()!,
            Subject        = reader["Subject"].ToString()!,
            FilePath       = reader["FilePath"].ToString()!,
            UploadedAt     = (DateTime)reader["UploadedAt"],
            FileSizeBytes  = (long)reader["FileSizeBytes"]
        });
    }

    return View(files.GroupBy(f => f.Subject));
}
```

---

## TASK 4: Implement `FilesController.cs` — Upload

**Replace the `Upload()` method body:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(IFormFile file, string subject)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    if (file == null || file.Length == 0)
    {
        TempData["Error"] = "Please select a file to upload.";
        return RedirectToAction("Index");
    }
    if (file.Length > 10 * 1024 * 1024)
    {
        TempData["Error"] = "File size must be less than 10 MB.";
        return RedirectToAction("Index");
    }

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

    string uploadFolder  = Path.Combine(_env.WebRootPath, "uploads", studentId.ToString());
    Directory.CreateDirectory(uploadFolder);

    string extension      = Path.GetExtension(file.FileName);
    string storedFileName = Guid.NewGuid().ToString() + extension;
    string fullPath       = Path.Combine(uploadFolder, storedFileName);
    string relativePath   = Path.Combine("uploads", studentId.ToString(), storedFileName);

    using (var stream = new FileStream(fullPath, FileMode.Create))
        await file.CopyToAsync(stream);

    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(@"
        INSERT INTO StudentFiles (StudentId, FileName, StoredFileName, Subject, FilePath, FileSizeBytes)
        VALUES (@sid, @fname, @sname, @subject, @path, @size)", conn);
    cmd.Parameters.AddWithValue("@sid",     studentId);
    cmd.Parameters.AddWithValue("@fname",   file.FileName);
    cmd.Parameters.AddWithValue("@sname",   storedFileName);
    cmd.Parameters.AddWithValue("@subject", subject);
    cmd.Parameters.AddWithValue("@path",    relativePath);
    cmd.Parameters.AddWithValue("@size",    file.Length);
    cmd.ExecuteNonQuery();

    TempData["Success"] = $"'{file.FileName}' uploaded to {subject}!";
    return RedirectToAction("Index");
}
```

---

## TASK 5: Implement `FilesController.cs` — Download

**Replace the `Download()` method body:**
```csharp
public IActionResult Download(int id)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

    using var conn = _db.GetConnection();
    using var cmd = new SqlCommand(
        "SELECT * FROM StudentFiles WHERE Id = @id AND StudentId = @sid", conn);
    cmd.Parameters.AddWithValue("@id",  id);
    cmd.Parameters.AddWithValue("@sid", studentId);
    using var reader = cmd.ExecuteReader();

    if (!reader.Read()) return NotFound();

    string fileName = reader["FileName"].ToString()!;
    string filePath = reader["FilePath"].ToString()!;
    string fullPath = Path.Combine(_env.WebRootPath, filePath);

    if (!System.IO.File.Exists(fullPath)) return NotFound();

    return PhysicalFile(fullPath, "application/octet-stream", fileName);
}
```

---

## TASK 6: Implement `FilesController.cs` — Delete

**Replace the `Delete()` method body:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Delete(int id)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

    using var conn = _db.GetConnection();
    using var selectCmd = new SqlCommand(
        "SELECT FilePath FROM StudentFiles WHERE Id = @id AND StudentId = @sid", conn);
    selectCmd.Parameters.AddWithValue("@id",  id);
    selectCmd.Parameters.AddWithValue("@sid", studentId);
    var filePath = selectCmd.ExecuteScalar()?.ToString();

    if (filePath == null) return NotFound();

    string fullPath = Path.Combine(_env.WebRootPath, filePath);
    if (System.IO.File.Exists(fullPath))
        System.IO.File.Delete(fullPath);

    using var deleteCmd = new SqlCommand(
        "DELETE FROM StudentFiles WHERE Id = @id", conn);
    deleteCmd.Parameters.AddWithValue("@id", id);
    deleteCmd.ExecuteNonQuery();

    TempData["Success"] = "File deleted.";
    return RedirectToAction("Index");
}
```

---

## TASK 7: Implement `SettingsController.cs` — ChangePassword

**File:** `Controllers/SettingsController.cs`

**Add this using at the top:**
```csharp
using Microsoft.Data.SqlClient;
```

**Replace the `ChangePassword()` method body:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult ChangePassword(ChangePasswordViewModel model)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    if (!ModelState.IsValid)
        return View("Index", model);

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");
    string currentHash = HashPassword(model.CurrentPassword);

    using var conn = _db.GetConnection();

    using var checkCmd = new SqlCommand(
        "SELECT COUNT(*) FROM Students WHERE Id = @id AND PasswordHash = @hash", conn);
    checkCmd.Parameters.AddWithValue("@id",   studentId);
    checkCmd.Parameters.AddWithValue("@hash", currentHash);
    if ((int)checkCmd.ExecuteScalar()! == 0)
    {
        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
        return View("Index", model);
    }

    using var updateCmd = new SqlCommand(
        "UPDATE Students SET PasswordHash = @hash WHERE Id = @id", conn);
    updateCmd.Parameters.AddWithValue("@hash", HashPassword(model.NewPassword));
    updateCmd.Parameters.AddWithValue("@id",   studentId);
    updateCmd.ExecuteNonQuery();

    TempData["Success"] = "Password changed successfully!";
    return RedirectToAction("Index");
}
```

---

## ✅ Test Part 2

1. `dotnet run`
2. Login → Files → upload a PDF → appears in list ✅
3. Download → file downloads ✅
4. Delete → file removed ✅
5. Settings → change password → login with new password ✅

## 📤 Push Part 2

```bash
git add .
git commit -m "feat: implement file management and settings - Het (assigned from Bhavya)"
git push origin bhavya
```

---

## 🔁 Create Pull Requests (both)

After both pushes, create **2 Pull Requests** on GitHub:

1. **PR 1:** `het` → `main` | Title: `feat: Attendance Dashboard + REST API - Het`
2. **PR 2:** `bhavya` → `main` | Title: `feat: File Management + Settings - Het`

Then message Tejas to review and merge both.

---

## 📂 Files You Touch

**On `het` branch:**
- `Controllers/DashboardController.cs`
- `Controllers/Api/AttendanceApiController.cs`

**On `bhavya` branch:**
- `Controllers/FilesController.cs`
- `Controllers/SettingsController.cs`

❌ Do NOT touch: Models, Views, Layout, site.css, Program.cs, AccountController, ProfileController
