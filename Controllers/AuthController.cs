using Microsoft.AspNetCore.Mvc;
using WebAssignment.DTOs;
using WebAssignment.Interfaces;

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
    }
}
