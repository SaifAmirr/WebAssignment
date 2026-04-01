namespace WebAssignment.DTOs
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int InstructorId { get; set; }
        public string? InstructorName { get; set; }
    }
}
