namespace WebAssignment.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? Grade { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Navigation properties
        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
