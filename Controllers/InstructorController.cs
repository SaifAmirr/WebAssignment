using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebAssignment.DTOs;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _service;

        public InstructorController(IInstructorService service)
        {
            _service = service;
        }

        // Endpoint 1 — Get All Instructors
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var instructors = await _service.GetAllAsync();
            return Ok(instructors);
        }

        // Endpoint 2 — Get Instructor by Id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var instructor = await _service.GetByIdAsync(id);
                return Ok(instructor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 3 — Add Instructor
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] InstructorCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var instructor = new Instructor
            {
                Name = dto.Name,
                Department = dto.Department,
                Email = dto.Email
            };

            await _service.AddAsync(instructor);
            
            var response = await _service.GetByIdAsync(instructor.Id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] InstructorUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var instructor = new Instructor
                {
                    Id = id,
                    Name = dto.Name,
                    Department = dto.Department,
                    Email = dto.Email
                };

                await _service.UpdateAsync(instructor);

                var updated = await _service.GetByIdAsync(id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 4 — Create or Update Instructor Profile
        [HttpPost("{id}/profile")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> CreateOrUpdateProfile(int id, [FromBody] InstructorProfileCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var profile = new InstructorProfile
                {
                    PhoneNumber = dto.PhoneNumber,
                    OfficeLocation = dto.OfficeLocation,
                    YearsOfExperience = dto.YearsOfExperience,
                    InstructorId = id
                };

                await _service.CreateOrUpdateProfileAsync(id, profile);
                return Ok("Profile created or updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/profile")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] InstructorProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var profileDto = await _service.GetProfileAsync(id);
                var profile = new InstructorProfile
                {
                    Id = profileDto.Id,
                    PhoneNumber = dto.PhoneNumber,
                    OfficeLocation = dto.OfficeLocation,
                    YearsOfExperience = dto.YearsOfExperience,
                    InstructorId = profileDto.InstructorId
                };

                await _service.UpdateProfileAsync(profile);

                var updated = await _service.GetProfileAsync(id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 5 — Get Instructor Profile
        [HttpGet("{id}/profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                var profile = await _service.GetProfileAsync(id);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 6 — Get All Courses for an Instructor
        [HttpGet("{id}/courses")]
        [Authorize]
        public async Task<IActionResult> GetInstructorCourses(int id)
        {
            try
            {
                var courses = await _service.GetInstructorCoursesAsync(id);
                return Ok(courses);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
