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
            var course = _service.GetById(id);

            if (course == null)
                return NotFound("Course not found");

            return Ok(course);
        }

        // Endpoint 3 — Add Course
        [HttpPost]
        public IActionResult Add(Course course)
        {
            _service.Add(course);
            return Ok(course);
        }
    }
}