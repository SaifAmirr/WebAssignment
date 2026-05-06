using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAssignment.DTOs;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthenticationService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        private void SetAuthCookie(string token)
        {
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // set true in production (requires HTTPS)
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.AuthenticateAsync(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            SetAuthCookie(_authService.GenerateJwtToken(user));

            return Ok(new LoginResponseDto
            {
                Username = user.Username,
                Role = user.Role.ToString(),
                StudentId = user.StudentId,
                InstructorId = user.InstructorId
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Role is always Pending on self-registration; admin assigns the real role later.
            var user = await _authService.CreateUserAsync(dto.Username, dto.Password, dto.Email, UserRole.Pending, null, null);
            if (user == null)
                return BadRequest("Username already exists.");

            return Ok(new { message = "Registration successful. Please log in." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var username    = User.FindFirstValue(ClaimTypes.Name);
            var role        = User.FindFirstValue(ClaimTypes.Role);
            var studentId   = User.FindFirstValue("StudentId");
            var instructorId = User.FindFirstValue("InstructorId");

            return Ok(new LoginResponseDto
            {
                Username = username ?? string.Empty,
                Role = role ?? string.Empty,
                StudentId = studentId != null ? int.Parse(studentId) : null,
                InstructorId = instructorId != null ? int.Parse(instructorId) : null
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return NoContent();
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            var result = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                InstructorId = u.InstructorId
            });
            return Ok(result);
        }

        [HttpPut("users/{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRoleUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
                return BadRequest("Invalid role. Must be Student, Instructor, or Admin.");

            var user = await _authService.UpdateUserRoleAsync(id, role);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                InstructorId = user.InstructorId
            });
        }
    }
}
