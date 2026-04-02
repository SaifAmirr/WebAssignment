namespace WebAssignment.Models
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CreditHours { get; set; }

        // Foreign Key for One-to-Many with Instructor
        public int InstructorId { get; set; }

        // One-to-Many relationship with Instructor
        public Instructor? Instructor { get; set; }

        // Many-to-Many relationship with Student through Enrollment
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}