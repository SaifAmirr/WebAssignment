using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class EnrollmentUpdateDto
    {
        [MaxLength(2)]
        [RegularExpression(@"^[A-F][-+]?$|^Pass$|^Fail$")]
        public string? Grade { get; set; }
    }
}
