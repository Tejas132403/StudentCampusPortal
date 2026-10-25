namespace StudentCampusPortal.Models
{
    // Represents a file uploaded by a student
    public class SubjectFile
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string FileName { get; set; } = string.Empty;       // Original file name shown to user
        public string StoredFileName { get; set; } = string.Empty; // Unique name stored on disk
        public string Subject { get; set; } = string.Empty;        // Folder/category e.g. "DAA"
        public string FilePath { get; set; } = string.Empty;       // Relative path inside wwwroot/uploads/
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public long FileSizeBytes { get; set; }
    }
}
