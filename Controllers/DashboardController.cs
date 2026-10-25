using Microsoft.AspNetCore.Mvc;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;

namespace StudentCampusPortal.Controllers
{
    /// <summary>
    /// ═══════════════════════════════════════════════════════════════
    /// ASSIGNED TO: HET
    /// BRANCH: het
    /// STATUS: STUB — implement all TODO sections below
    ///
    /// PURPOSE: Show subject-wise attendance on the dashboard.
    ///
    /// DEPENDENCY: This controller needs the base project (from main
    /// branch) to be already set up by Tejas before you start.
    /// Pull from main first: git pull origin main
    /// ═══════════════════════════════════════════════════════════════
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly DatabaseHelper _db;

        public DashboardController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: /Dashboard  (also the home page after login)
        public IActionResult Index()
        {
            // Auth check — if not logged in, redirect to login page
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            int studentId = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0");

            // TODO (HET): Query all attendance rows for this student from the DB
            // Use DatabaseHelper.GetConnection() to get a SqlConnection
            // Query: SELECT * FROM Attendance WHERE StudentId = @studentId
            // Map each row to an AttendanceRecord object
            // Return the list as the View model
            //
            // EXAMPLE STRUCTURE (replace this placeholder list):
            var records = new List<AttendanceRecord>();

            // using var conn = _db.GetConnection();
            // using var cmd = new SqlCommand("SELECT * FROM Attendance WHERE StudentId=@id", conn);
            // cmd.Parameters.AddWithValue("@id", studentId);
            // using var reader = cmd.ExecuteReader();
            // while (reader.Read()) {
            //     records.Add(new AttendanceRecord {
            //         Id = (int)reader["Id"],
            //         StudentId = studentId,
            //         Subject = reader["Subject"].ToString()!,
            //         Type = reader["Type"].ToString()!,
            //         TotalClasses = (int)reader["TotalClasses"],
            //         AttendedClasses = (int)reader["AttendedClasses"]
            //     });
            // }

            return View(records);
        }
    }
}
