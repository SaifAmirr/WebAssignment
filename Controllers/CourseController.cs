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

        // Endpoint 2 — Get Course by Id
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                var course = _service.GetById(id);
                var response = new CourseResponseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    CreditHours = course.CreditHours,
                    InstructorId = course.InstructorId,
                    InstructorName = course.Instructor?.Name
                };
                return Ok(response);
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

            Instructor instructor = _instructorService.GetById(dto.InstructorId);
            
            if (instructor == null)
            {
                return NotFound($"Instructor with Id {dto.InstructorId} not found");
            }
                       
            var course = new Course
            {
                Title = dto.Title,
                CreditHours = dto.CreditHours,
                InstructorId = dto.InstructorId,
                Instructor = instructor
            };

            _service.Add(course);
            
            var response = new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                CreditHours = course.CreditHours,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor?.Name
            };
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
                var response = students.Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    GPA = s.GPA
                }).ToList();
                return Ok(response);
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