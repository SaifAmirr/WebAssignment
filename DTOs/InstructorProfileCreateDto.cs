using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class InstructorProfileCreateDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string OfficeLocation { get; set; } = string.Empty;

        [Required]
        [Range(0, 70)]
        public int YearsOfExperience { get; set; }
    }
}
