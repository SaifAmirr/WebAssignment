using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.AuthenticateAsync(dto.Username, dto.Password);
            
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _authService.GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse("60"));

            var response = new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                ExpiresAt = expiresAt
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            {
                return BadRequest("Invalid role. Must be Student, Instructor, or Admin.");
            }

            var user = await _authService.CreateUserAsync(dto.Username, dto.Password, dto.Email, role, null, null);

            if (user == null)
            {
                return BadRequest("Username already exists.");
            }

            var response = new LoginResponseDto
            {
                Token = _authService.GenerateJwtToken(user),
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse("60"))
            };

            return Ok(response);
        }
    }
}
