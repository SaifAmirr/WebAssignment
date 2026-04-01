using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class StudentUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.0, 4.0)]
        public double GPA { get; set; }
    }
}
