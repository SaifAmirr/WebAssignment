namespace WebAssignment.Models
{
    public class Student
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double GPA { get; set; }

        // Many-to-Many relationship with Course through Enrollment
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}