namespace WebAssignment.DTOs
{
    public class StudentResponseDto
    {
        public int Id { get; set; }
        public int? StudentNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public double GPA { get; set; }
    }
}
