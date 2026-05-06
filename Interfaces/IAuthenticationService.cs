using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        string GenerateJwtToken(User user);
        Task<User?> CreateUserAsync(string username, string password, string email, UserRole role, int? studentId, int? instructorId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> UpdateUserRoleAsync(int userId, UserRole role);
        Task<bool> LinkUserToInstructorAsync(int userId, int instructorId);
    }
}
