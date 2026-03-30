using Microsoft.AspNetCore.Mvc;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService service)
        {
            _service = service;
        }

        // Endpoint 1 — Get All Courses
        [HttpGet]
        public IActionResult GetAll()
        {
            var courses = _service.GetAll();
            return Ok(courses);
        }

        // Endpoint 2 — Get Course by Id
        [HttpGet("{id}")]
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

        // Endpoint 3 — Add Course
        [HttpPost]
        public IActionResult Add(Course course)
        {
            _service.Add(course);
            return Ok(course);
        }

        // Endpoint 4 — Get All Enrolled Students for a Course
        [HttpGet("{id}/enrollments")]
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