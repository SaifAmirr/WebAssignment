namespace WebAssignment.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Department { get; set; }
        public required string Email { get; set; }

        // One-to-One relationship with InstructorProfile
        public InstructorProfile? InstructorProfile { get; set; }

        // One-to-Many relationship with Course
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
