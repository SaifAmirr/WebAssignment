namespace WebAssignment.Models
{
    public class InstructorProfile
    {
        public int Id { get; set; }
        public required string PhoneNumber { get; set; }
        public required string OfficeLocation { get; set; }
        public int YearsOfExperience { get; set; }

        // Foreign Key
        public int InstructorId { get; set; }

        // One-to-One relationship with Instructor
        public required Instructor Instructor { get; set; }
    }
}
