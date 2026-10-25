using Microsoft.AspNetCore.Mvc;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;

namespace StudentCampusPortal.Controllers
{
    /// <summary>
    /// DONE BY: TEJAS (branch: tejas)
    /// Handles: Student profile page
    /// Shows name, enrollment, class, batch of the logged-in student.
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly DatabaseHelper _db;

        public ProfileController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: /Profile
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            // Read student info from session
            var student = new Student
            {
                Id           = int.Parse(HttpContext.Session.GetString("StudentId") ?? "0"),
                Name         = HttpContext.Session.GetString("StudentName") ?? "",
                EnrollmentNo = HttpContext.Session.GetString("EnrollmentNo") ?? "",
                Class        = HttpContext.Session.GetString("Class") ?? "",
                Batch        = HttpContext.Session.GetString("Batch") ?? ""
            };

            return View(student);
        }
    }
}
