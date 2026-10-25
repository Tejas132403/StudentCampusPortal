using Microsoft.Data.SqlClient;

namespace StudentCampusPortal.Data
{
    /// <summary>
    /// ADO.NET helper — single place to get DB connections.
    /// All controllers use this class. Connection string is read from appsettings.json.
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>Returns an open SqlConnection. Caller must dispose it (use in 'using' block).</summary>
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Runs the database setup SQL — creates tables if they don't exist.
        /// Called once on app startup from Program.cs.
        /// </summary>
        public void InitializeDatabase()
        {
            string sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Students' AND xtype='U')
                CREATE TABLE Students (
                    Id          INT PRIMARY KEY IDENTITY(1,1),
                    Name        NVARCHAR(100)  NOT NULL,
                    EnrollmentNo NVARCHAR(20)  NOT NULL UNIQUE,
                    Email       NVARCHAR(100)  NOT NULL,
                    PasswordHash NVARCHAR(256) NOT NULL,
                    Class       NVARCHAR(10)   NOT NULL,
                    Batch       NVARCHAR(5)    NOT NULL
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Attendance' AND xtype='U')
                CREATE TABLE Attendance (
                    Id              INT PRIMARY KEY IDENTITY(1,1),
                    StudentId       INT           NOT NULL REFERENCES Students(Id),
                    Subject         NVARCHAR(50)  NOT NULL,
                    Type            NVARCHAR(20)  NOT NULL,  -- 'Theory' or 'Lab'
                    TotalClasses    INT           NOT NULL DEFAULT 0,
                    AttendedClasses INT           NOT NULL DEFAULT 0
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StudentFiles' AND xtype='U')
                CREATE TABLE StudentFiles (
                    Id              INT PRIMARY KEY IDENTITY(1,1),
                    StudentId       INT            NOT NULL REFERENCES Students(Id),
                    FileName        NVARCHAR(200)  NOT NULL,
                    StoredFileName  NVARCHAR(200)  NOT NULL,
                    Subject         NVARCHAR(50)   NOT NULL,
                    FilePath        NVARCHAR(500)  NOT NULL,
                    UploadedAt      DATETIME       NOT NULL DEFAULT GETDATE(),
                    FileSizeBytes   BIGINT         NOT NULL DEFAULT 0
                );
            ";

            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Seeds demo attendance data for a student if they have none.
        /// Het will call this for new registrations.
        /// </summary>
        public void SeedAttendanceForStudent(int studentId)
        {
            using var conn = GetConnection();

            // Check if data already exists
            using var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM Attendance WHERE StudentId = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", studentId);
            int count = (int)checkCmd.ExecuteScalar()!;
            if (count > 0) return;

            // Seed demo subjects
            var subjects = new[]
            {
                (".NET", "Theory", 40, 34),
                (".NET", "Lab",    20, 18),
                ("DAA",  "Theory", 45, 38),
                ("DAA",  "Lab",    20, 17),
                ("FM",   "Theory", 40, 33),
                ("FM",   "Lab",    20, 16),
                ("CCE",  "Theory", 38, 32),
                ("CCE",  "Lab",    18, 18),
            };

            foreach (var (subject, type, total, attended) in subjects)
            {
                using var cmd = new SqlCommand(@"
                    INSERT INTO Attendance (StudentId, Subject, Type, TotalClasses, AttendedClasses)
                    VALUES (@sid, @sub, @type, @total, @attended)", conn);
                cmd.Parameters.AddWithValue("@sid",      studentId);
                cmd.Parameters.AddWithValue("@sub",      subject);
                cmd.Parameters.AddWithValue("@type",     type);
                cmd.Parameters.AddWithValue("@total",    total);
                cmd.Parameters.AddWithValue("@attended", attended);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
