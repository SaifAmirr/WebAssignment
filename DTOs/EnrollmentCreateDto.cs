using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class EnrollmentCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
