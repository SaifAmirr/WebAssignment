using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebAssignment.DTOs;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;
        private readonly IInstructorService _instructorService;

        public CourseController(ICourseService service, IInstructorService instructorService)
        {
            _service = service;
            _instructorService = instructorService;
        }

        // Endpoint 1 — Get All Courses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _service.GetAllAsync();
            return Ok(courses);
        }

        // Endpoint 2 — Get Course by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var course = await _service.GetByIdAsync(id);
                return Ok(course);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validate instructor exists
                try
                {
                    await _instructorService.GetByIdAsync(dto.InstructorId);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound($"Instructor with Id {dto.InstructorId} not found");
                }

                var course = new Course
                {
                    Id = id,
                    Title = dto.Title,
                    CreditHours = dto.CreditHours,
                    InstructorId = dto.InstructorId
                };

                await _service.UpdateAsync(course);

                var updated = await _service.GetByIdAsync(id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 3 — Add Course
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate instructor exists by trying to fetch it
            try
            {
                await _instructorService.GetByIdAsync(dto.InstructorId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Instructor with Id {dto.InstructorId} not found");
            }
                       
            var course = new Course
            {
                Title = dto.Title,
                CreditHours = dto.CreditHours,
                InstructorId = dto.InstructorId
            };

            await _service.AddAsync(course);
            
            var response = await _service.GetByIdAsync(course.Id);
            return Ok(response);
        }

        // Endpoint 5 — Assign Instructor to Course
        [HttpPut("{courseId}/instructor/{instructorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignInstructor(int courseId, int instructorId)
        {
            try
            {
                await _service.AssignInstructorAsync(courseId, instructorId);
                return Ok("Instructor assigned successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}