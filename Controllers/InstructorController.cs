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
        public IActionResult GetAll()
        {
            var instructors = _service.GetAll();
            return Ok(instructors);
        }

        // Endpoint 2 — Get Instructor by Id
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                var instructor = _service.GetById(id);
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
        public IActionResult Add([FromBody] InstructorCreateDto dto)
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

            _service.Add(instructor);
            
            var response = _service.GetById(instructor.Id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Update(int id, [FromBody] InstructorUpdateDto dto)
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

                _service.Update(instructor);

                var updated = _service.GetById(id);
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
        public IActionResult CreateOrUpdateProfile(int id, [FromBody] InstructorProfileCreateDto dto)
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

                _service.CreateOrUpdateProfile(id, profile);
                return Ok("Profile created or updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/profile")]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult UpdateProfile(int id, [FromBody] InstructorProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var profileDto = _service.GetProfile(id);
                var profile = new InstructorProfile
                {
                    Id = profileDto.Id,
                    PhoneNumber = dto.PhoneNumber,
                    OfficeLocation = dto.OfficeLocation,
                    YearsOfExperience = dto.YearsOfExperience,
                    InstructorId = profileDto.InstructorId
                };

                _service.UpdateProfile(profile);

                var updated = _service.GetProfile(id);
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
        public IActionResult GetProfile(int id)
        {
            try
            {
                var profile = _service.GetProfile(id);
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
        public IActionResult GetInstructorCourses(int id)
        {
            try
            {
                var courses = _service.GetInstructorCourses(id);
                return Ok(courses);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
