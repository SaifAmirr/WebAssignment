using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class InstructorCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int? UserId { get; set; }
    }
}
