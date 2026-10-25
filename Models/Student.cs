namespace StudentCampusPortal.Models
{
    // Represents a registered student in the portal
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EnrollmentNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;   // e.g. "5EC2"
        public string Batch { get; set; } = string.Empty;   // e.g. "A"
    }
}
