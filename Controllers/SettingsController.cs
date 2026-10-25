using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentCampusPortal.Controllers
{
    /// <summary>
    /// ═══════════════════════════════════════════════════════════════
    /// ASSIGNED TO: BHAVYA
    /// BRANCH: bhavya
    /// STATUS: STUB — implement all TODO sections below
    ///
    /// PURPOSE: Settings page — change password, view profile info, logout.
    /// ═══════════════════════════════════════════════════════════════
    /// </summary>
    public class SettingsController : Controller
    {
        private readonly DatabaseHelper _db;

        public SettingsController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: /Settings
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            return View(new ChangePasswordViewModel());
        }

        // POST: /Settings/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (HttpContext.Session.GetString("EnrollmentNo") == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View("Index", model);

            // TODO (BHAVYA): Implement change password
            // 1. Get studentId from session
            // 2. Hash the CurrentPassword using SHA256 (same as AccountController.HashPassword)
            // 3. SELECT COUNT(*) FROM Students WHERE Id=@id AND PasswordHash=@currentHash
            //    If count == 0 → ModelState.AddModelError("CurrentPassword", "Current password is wrong") → return View
            // 4. Hash the NewPassword
            // 5. UPDATE Students SET PasswordHash=@newHash WHERE Id=@id
            // 6. TempData["Success"] = "Password changed successfully!"
            // 7. return RedirectToAction("Index")

            TempData["Error"] = "Change password not implemented yet.";
            return View("Index", model);
        }

        // Helper (copy of AccountController version — kept here to avoid dependency)
        private static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
