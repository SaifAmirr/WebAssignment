namespace WebAssignment.DTOs
{
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? Grade { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
    }
}
