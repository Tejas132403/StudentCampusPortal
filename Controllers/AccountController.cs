using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentCampusPortal.Controllers
{
    /// <summary>
    /// DONE BY: TEJAS (branch: tejas)
    /// Handles: Login, Register, Logout
    /// Uses: Session for auth, Cookies for remember-me
    /// Lab coverage: Lab 4 (Validation), Lab 5 (Session + Cookies)
    /// </summary>
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;

        public AccountController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("EnrollmentNo") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string hash = HashPassword(model.Password);

            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT Id, Name, EnrollmentNo, Class, Batch FROM Students WHERE EnrollmentNo=@e AND PasswordHash=@p",
                conn);
            cmd.Parameters.AddWithValue("@e", model.EnrollmentNo);
            cmd.Parameters.AddWithValue("@p", hash);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Store student info in session (Lab 5)
                HttpContext.Session.SetString("StudentId",    reader["Id"].ToString()!);
                HttpContext.Session.SetString("StudentName",  reader["Name"].ToString()!);
                HttpContext.Session.SetString("EnrollmentNo", reader["EnrollmentNo"].ToString()!);
                HttpContext.Session.SetString("Class",        reader["Class"].ToString()!);
                HttpContext.Session.SetString("Batch",        reader["Batch"].ToString()!);

                // Remember me cookie (Lab 5)
                if (model.RememberMe)
                {
                    CookieOptions opt = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true
                    };
                    Response.Cookies.Append("RememberEnrollment", model.EnrollmentNo, opt);
                }

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid enrollment number or password.");
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var conn = _db.GetConnection();

                // Check if enrollment already exists
                using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Students WHERE EnrollmentNo=@e", conn);
                checkCmd.Parameters.AddWithValue("@e", model.EnrollmentNo);
                int existing = (int)checkCmd.ExecuteScalar()!;
                if (existing > 0)
                {
                    ModelState.AddModelError("EnrollmentNo", "This enrollment number is already registered.");
                    return View(model);
                }

                // Insert new student
                using var cmd = new SqlCommand(@"
                    INSERT INTO Students (Name, EnrollmentNo, Email, PasswordHash, Class, Batch)
                    OUTPUT INSERTED.Id
                    VALUES (@name, @enroll, @email, @pass, @class, @batch)", conn);
                cmd.Parameters.AddWithValue("@name",   model.Name);
                cmd.Parameters.AddWithValue("@enroll", model.EnrollmentNo);
                cmd.Parameters.AddWithValue("@email",  model.Email);
                cmd.Parameters.AddWithValue("@pass",   HashPassword(model.Password));
                cmd.Parameters.AddWithValue("@class",  model.Class);
                cmd.Parameters.AddWithValue("@batch",  model.Batch);

                int newId = (int)cmd.ExecuteScalar()!;

                // Seed demo attendance data for new student
                _db.SeedAttendanceForStudent(newId);

                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Registration failed: " + ex.Message);
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("RememberEnrollment");
            return RedirectToAction("Login");
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
