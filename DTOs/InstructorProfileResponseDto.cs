namespace WebAssignment.DTOs
{
    public class InstructorProfileResponseDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string OfficeLocation { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public int InstructorId { get; set; }
    }
}
