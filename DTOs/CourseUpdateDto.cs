using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class CourseUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int CreditHours { get; set; }

        [Required]
        public int InstructorId { get; set; }
    }
}
