# 🟠 BHAVYA's Task — File Management + Settings
### Branch: `bhavya` | Assigned to: Bhavya | Estimated time: 3-4 hours

---

## ⚠️ BEFORE YOU START — Read This First

**Step 1: Wait for Tejas to push the base project to `main`.** (He will message you when done.)

**Step 2: Clone the repo and checkout YOUR branch:**
```bash
git clone https://github.com/<REPO-URL>/StudentCampusPortal.git
cd StudentCampusPortal
git checkout bhavya
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

**Step 5: Register a test account and login. Click "Files" in the sidebar — it should load (even if empty).**

✅ Only then start coding.

---

## 📋 Your Tasks (in order)

### TASK 1: Implement `FilesController.cs` — Index (List Files)

**File:** `Controllers/FilesController.cs`

Find the `// TODO (BHAVYA):` comment in the `Index()` method and replace it:

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

    var grouped = files.GroupBy(f => f.Subject);
    return View(grouped);
}
```

**Add this using at the top of the file:**
```csharp
using Microsoft.Data.SqlClient;
```

---

### TASK 2: Implement `FilesController.cs` — Upload

Find the `// TODO (BHAVYA):` comment in the `Upload()` method and replace it:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(IFormFile file, string subject)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    // Validate file
    if (file == null || file.Length == 0)
    {
        TempData["Error"] = "Please select a file to upload.";
        return RedirectToAction("Index");
    }
    if (file.Length > 10 * 1024 * 1024) // 10 MB limit
    {
        TempData["Error"] = "File size must be less than 10 MB.";
        return RedirectToAction("Index");
    }

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

    // Create student's upload folder
    string uploadFolder = Path.Combine(_env.WebRootPath, "uploads", studentId.ToString());
    Directory.CreateDirectory(uploadFolder);

    // Generate unique filename to avoid conflicts
    string extension      = Path.GetExtension(file.FileName);
    string storedFileName = Guid.NewGuid().ToString() + extension;
    string fullPath       = Path.Combine(uploadFolder, storedFileName);
    string relativePath   = Path.Combine("uploads", studentId.ToString(), storedFileName);

    // Save file to disk
    using (var stream = new FileStream(fullPath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // Save file record to database
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

    TempData["Success"] = $"'{file.FileName}' uploaded successfully to {subject}!";
    return RedirectToAction("Index");
}
```

---

### TASK 3: Implement `FilesController.cs` — Download

Find the `// TODO (BHAVYA):` in `Download()` and replace it:

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

    if (!reader.Read())
        return NotFound();

    string fileName   = reader["FileName"].ToString()!;
    string filePath   = reader["FilePath"].ToString()!;
    string fullPath   = Path.Combine(_env.WebRootPath, filePath);

    if (!System.IO.File.Exists(fullPath))
        return NotFound();

    return PhysicalFile(fullPath, "application/octet-stream", fileName);
}
```

---

### TASK 4: Implement `FilesController.cs` — Delete

Find the `// TODO (BHAVYA):` in `Delete()` and replace it:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Delete(int id)
{
    if (HttpContext.Session.GetString("EnrollmentNo") == null)
        return RedirectToAction("Login", "Account");

    int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

    using var conn = _db.GetConnection();

    // Get file info first
    using var selectCmd = new SqlCommand(
        "SELECT FilePath FROM StudentFiles WHERE Id = @id AND StudentId = @sid", conn);
    selectCmd.Parameters.AddWithValue("@id",  id);
    selectCmd.Parameters.AddWithValue("@sid", studentId);
    var filePath = selectCmd.ExecuteScalar()?.ToString();

    if (filePath == null) return NotFound();

    // Delete physical file
    string fullPath = Path.Combine(_env.WebRootPath, filePath);
    if (System.IO.File.Exists(fullPath))
        System.IO.File.Delete(fullPath);

    // Delete DB record
    using var deleteCmd = new SqlCommand(
        "DELETE FROM StudentFiles WHERE Id = @id", conn);
    deleteCmd.Parameters.AddWithValue("@id", id);
    deleteCmd.ExecuteNonQuery();

    TempData["Success"] = "File deleted successfully.";
    return RedirectToAction("Index");
}
```

---

### TASK 5: Implement `SettingsController.cs` — ChangePassword

**File:** `Controllers/SettingsController.cs`

Find the `// TODO (BHAVYA):` in `ChangePassword()` and replace it:

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

    // Verify current password
    using var checkCmd = new SqlCommand(
        "SELECT COUNT(*) FROM Students WHERE Id = @id AND PasswordHash = @hash", conn);
    checkCmd.Parameters.AddWithValue("@id",   studentId);
    checkCmd.Parameters.AddWithValue("@hash", currentHash);
    int count = (int)checkCmd.ExecuteScalar()!;

    if (count == 0)
    {
        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
        return View("Index", model);
    }

    // Update with new password
    string newHash = HashPassword(model.NewPassword);
    using var updateCmd = new SqlCommand(
        "UPDATE Students SET PasswordHash = @hash WHERE Id = @id", conn);
    updateCmd.Parameters.AddWithValue("@hash", newHash);
    updateCmd.Parameters.AddWithValue("@id",   studentId);
    updateCmd.ExecuteNonQuery();

    TempData["Success"] = "Password changed successfully!";
    return RedirectToAction("Index");
}
```

**Add this using at the top of `SettingsController.cs`:**
```csharp
using Microsoft.Data.SqlClient;
```

---

## ✅ How to Test Your Work

1. Run the project: `dotnet run`
2. Register an account and login
3. Click **Files** in sidebar
4. Upload a PDF or any file — it should appear in the list grouped by subject
5. Click Download — file should download
6. Click the trash icon — file should be deleted
7. Click **Settings** in sidebar
8. Try changing password — login with new password to confirm it works

---

## 📤 How to Push Your Code

```bash
# Stage all your changes
git add .

# Commit with your name
git commit -m "feat: implement file management and settings - Bhavya"

# Push to YOUR branch (never push to main directly)
git push origin bhavya
```

---

## 🔁 How to Create a Pull Request

1. Go to GitHub repo in browser
2. Click **"Compare & pull request"** (appears after push)
3. Set: **base = main**, **compare = bhavya**
4. Title: `feat: File Management + Settings - Bhavya`
5. Description: What you built
6. Click **"Create pull request"**
7. Message Tejas to review and merge

---

## 📂 Files You Should Touch

✅ `Controllers/FilesController.cs` — implement Upload, Index, Download, Delete  
✅ `Controllers/SettingsController.cs` — implement ChangePassword  
❌ Do NOT touch: AccountController, DashboardController, any Views, Models, Layout, site.css

---

## 🚨 Common Mistakes to Avoid

- Do NOT push to `main` branch
- Do NOT edit `_Layout.cshtml` or `site.css`
- Always use `using var conn = _db.GetConnection()` (never forget the `using`)
- Make sure `wwwroot/uploads/` folder exists (it's already created in the base project)
- If upload fails, check that the `uploads` folder has write permissions
