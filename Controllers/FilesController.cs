using Microsoft.AspNetCore.Mvc;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;

namespace StudentCampusPortal.Controllers
{
    /// <summary>
    /// ═══════════════════════════════════════════════════════════════
    /// ASSIGNED TO: BHAVYA
    /// BRANCH: bhavya
    /// STATUS: STUB — implement all TODO sections below
    ///
    /// PURPOSE: Let students upload, view, and download study files.
    ///          Files are grouped by subject (like folders).
    ///
    /// DEPENDENCY: This controller needs the base project (from main
    /// branch) to be set up by Tejas before you start.
    /// Pull from main first: git pull origin main
    /// ═══════════════════════════════════════════════════════════════
    /// </summary>
    public class FilesController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IWebHostEnvironment _env;

        public FilesController(DatabaseHelper db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: /Files
        // Lists all files uploaded by the logged-in student, grouped by Subject
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

            // TODO (BHAVYA): Query all files for this student from the DB
            // Query: SELECT * FROM StudentFiles WHERE StudentId = @studentId ORDER BY Subject, UploadedAt DESC
            // Map rows to SubjectFile objects
            // Group them by Subject using .GroupBy(f => f.Subject)
            // Pass grouped files to the View
            //
            // The View expects: IEnumerable<IGrouping<string, SubjectFile>>

            var files = new List<SubjectFile>(); // replace with real DB query

            var grouped = files.GroupBy(f => f.Subject);
            return View(grouped);
        }

        // POST: /Files/Upload
        // Handles file upload from the form in Index.cshtml
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string subject)
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            // TODO (BHAVYA): Implement file upload
            // 1. Validate: file must not be null, size <= 10 MB
            // 2. Create folder: wwwroot/uploads/{studentId}/
            // 3. Generate a unique stored filename: Guid.NewGuid() + extension
            // 4. Save file to disk using: System.IO.File.CopyTo(...)
            // 5. Insert a row into StudentFiles table with:
            //    - StudentId, FileName (original), StoredFileName, Subject, FilePath, FileSizeBytes
            // 6. Redirect back to Index with TempData["Success"] = "File uploaded!"

            TempData["Error"] = "Upload not implemented yet.";
            return RedirectToAction("Index");
        }

        // GET: /Files/Download/{id}
        // Serves the file for download
        public IActionResult Download(int id)
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

            // TODO (BHAVYA): Implement file download
            // 1. Query StudentFiles WHERE Id=@id AND StudentId=@studentId (security: only own files)
            // 2. Build full path: Path.Combine(_env.WebRootPath, record.FilePath)
            // 3. Return PhysicalFile(fullPath, "application/octet-stream", record.FileName)
            //    This forces browser to download with the original file name

            return NotFound();
        }

        // POST: /Files/Delete/{id}
        // Deletes a file (from disk + DB)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

            // TODO (BHAVYA): Implement file deletion
            // 1. Query StudentFiles WHERE Id=@id AND StudentId=@studentId
            // 2. Delete physical file from disk using System.IO.File.Delete(fullPath)
            // 3. DELETE row from StudentFiles WHERE Id=@id
            // 4. Redirect with TempData["Success"] = "File deleted."

            TempData["Error"] = "Delete not implemented yet.";
            return RedirectToAction("Index");
        }
    }
}
