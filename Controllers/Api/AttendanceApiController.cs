using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentCampusPortal.Data;
using StudentCampusPortal.Models;

namespace StudentCampusPortal.Controllers.Api
{
    /// <summary>
    /// ═══════════════════════════════════════════════════════════════
    /// ASSIGNED TO: HET
    /// BRANCH: het
    /// Lab Coverage: Lab 10 — ASP.NET Core REST API
    ///
    /// This is a REST API controller (not MVC — no views).
    /// It returns JSON data for attendance.
    ///
    /// Test URL after running: GET /api/attendance/92400103122
    /// ═══════════════════════════════════════════════════════════════
    /// </summary>
    [Route("api/attendance")]
    [ApiController]
    public class AttendanceApiController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public AttendanceApiController(DatabaseHelper db)
        {
            _db = db;
        }

        /// <summary>
        /// GET /api/attendance/{enrollmentNo}
        /// Returns all attendance records for a student as JSON.
        /// Example: GET /api/attendance/92400103122
        /// </summary>
        [HttpGet("{enrollmentNo}")]
        public IActionResult GetByEnrollment(string enrollmentNo)
        {
            // TODO (HET): Implement this API endpoint
            // 1. Query Students WHERE EnrollmentNo=@e → get studentId
            //    If not found → return NotFound(new { message = "Student not found" })
            // 2. Query all Attendance rows for that studentId
            // 3. Map to a list of AttendanceRecord objects
            // 4. Also compute overall attendance % (total attended / total classes * 100)
            // 5. Return Ok(new { student = enrollmentNo, overall = overallPct, subjects = records })
            //
            // HINT — query for student:
            // SELECT Id FROM Students WHERE EnrollmentNo=@e
            //
            // HINT — query for attendance:
            // SELECT Subject, Type, TotalClasses, AttendedClasses FROM Attendance WHERE StudentId=@id

            return Ok(new { message = "API not implemented yet by Het" });
        }

        /// <summary>
        /// GET /api/attendance/all
        /// Returns attendance for ALL students (for demo/testing).
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            // TODO (HET): Optional — return attendance for all students
            // Useful for teacher/demo purposes
            // SELECT s.Name, s.EnrollmentNo, a.Subject, a.Type, a.TotalClasses, a.AttendedClasses
            // FROM Students s JOIN Attendance a ON s.Id = a.StudentId

            return Ok(new { message = "GetAll not implemented yet by Het" });
        }
    }
}
