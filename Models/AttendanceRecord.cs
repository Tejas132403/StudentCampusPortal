namespace StudentCampusPortal.Models
{
    // Represents attendance of one student for one subject (Theory OR Lab)
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;        // e.g. "DAA"
        public string Type { get; set; } = string.Empty;           // "Theory" or "Lab"
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }

        // Computed percentage — DO NOT store in DB
        public double Percentage => TotalClasses == 0
            ? 0
            : Math.Round((double)AttendedClasses / TotalClasses * 100, 1);
    }
}
