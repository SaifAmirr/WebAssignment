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
        [Authorize]
        public IActionResult GetAll()
        {
            var courses = _service.GetAll();
            return Ok(courses);
        }

        // Endpoint 2 — Get Course by Id
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                var course = _service.GetById(id);
                return Ok(course);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult Update(int id, [FromBody] CourseUpdateDto dto)
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
                    _instructorService.GetById(dto.InstructorId);
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

                _service.Update(course);

                var updated = _service.GetById(id);
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
        public IActionResult Add([FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate instructor exists by trying to fetch it
            try
            {
                _instructorService.GetById(dto.InstructorId);
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

            _service.Add(course);
            
            var response = _service.GetById(course.Id);
            return Ok(response);
        }

        // Endpoint 4 — Get All Enrolled Students for a Course
        [HttpGet("{id}/enrollments")]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult GetCourseEnrollments(int id)
        {
            try
            {
                var students = _service.GetCourseEnrollments(id);
                return Ok(students);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 5 — Assign Instructor to Course
        [HttpPut("{courseId}/instructor/{instructorId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignInstructor(int courseId, int instructorId)
        {
            try
            {
                _service.AssignInstructor(courseId, instructorId);
                return Ok("Instructor assigned successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}