using System.ComponentModel.DataAnnotations;

namespace WebAssignment.DTOs
{
    public class UserRoleUpdateDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
