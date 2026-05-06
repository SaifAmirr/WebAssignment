using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class StudentCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Student number must be a positive integer.")]
        public int StudentNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.0, 4.0)]
        public double GPA { get; set; }
    }
}
