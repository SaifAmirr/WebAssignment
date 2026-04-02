namespace WebAssignment.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public UserRole Role { get; set; }
        public int? StudentId { get; set; }
        public int? InstructorId { get; set; }
    }
}
