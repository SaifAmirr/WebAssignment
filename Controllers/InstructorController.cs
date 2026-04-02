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
            var response = instructors.Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Department = i.Department,
                Email = i.Email
            }).ToList();
            return Ok(response);
        }

        // Endpoint 2 — Get Instructor by Id
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                var instructor = _service.GetById(id);
                var response = new InstructorResponseDto
                {
                    Id = instructor.Id,
                    Name = instructor.Name,
                    Department = instructor.Department,
                    Email = instructor.Email
                };
                return Ok(response);
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
            
            var response = new InstructorResponseDto
            {
                Id = instructor.Id,
                Name = instructor.Name,
                Department = instructor.Department,
                Email = instructor.Email
            };
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
                var instructor = _service.GetById(id);
                instructor.Name = dto.Name;
                instructor.Department = dto.Department;
                instructor.Email = dto.Email;

                _service.Update(instructor);

                var response = new InstructorResponseDto
                {
                    Id = instructor.Id,
                    Name = instructor.Name,
                    Department = instructor.Department,
                    Email = instructor.Email
                };
                return Ok(response);
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

            Instructor instructor = _service.GetById(id);
            
            if (instructor == null)
            {
                return NotFound($"Instructor with Id {id} not found");
            }

            var profile = new InstructorProfile
            {
                PhoneNumber = dto.PhoneNumber,
                OfficeLocation = dto.OfficeLocation,
                YearsOfExperience = dto.YearsOfExperience,
                InstructorId = id,
                Instructor = instructor
            };

            _service.CreateOrUpdateProfile(id, profile);
            return Ok("Profile created or updated successfully");
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
                var profile = _service.GetProfile(id);
                profile.PhoneNumber = dto.PhoneNumber;
                profile.OfficeLocation = dto.OfficeLocation;
                profile.YearsOfExperience = dto.YearsOfExperience;

                _service.UpdateProfile(profile);

                var response = new InstructorProfileResponseDto
                {
                    Id = profile.Id,
                    PhoneNumber = profile.PhoneNumber,
                    OfficeLocation = profile.OfficeLocation,
                    YearsOfExperience = profile.YearsOfExperience,
                    InstructorId = profile.InstructorId
                };
                return Ok(response);
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
                var response = new InstructorProfileResponseDto
                {
                    Id = profile.Id,
                    PhoneNumber = profile.PhoneNumber,
                    OfficeLocation = profile.OfficeLocation,
                    YearsOfExperience = profile.YearsOfExperience,
                    InstructorId = profile.InstructorId
                };
                return Ok(response);
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
                var response = courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor?.Name
                }).ToList();
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
