using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        string GenerateJwtToken(User user);
        Task<User?> CreateUserAsync(string username, string password, string email, UserRole role, int? studentId, int? instructorId);
    }
}
