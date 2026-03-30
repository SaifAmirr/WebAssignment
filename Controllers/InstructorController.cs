using Microsoft.AspNetCore.Mvc;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _service;

        public InstructorController(IInstructorService service)
        {
            _service = service;
        }

        // Endpoint 1 — Get All Instructors
        [HttpGet]
        public IActionResult GetAll()
        {
            var instructors = _service.GetAll();
            return Ok(instructors);
        }

        // Endpoint 2 — Get Instructor by Id
        [HttpGet("{id}")]
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
        public IActionResult Add(Instructor instructor)
        {
            _service.Add(instructor);
            return Ok(instructor);
        }

        // Endpoint 4 — Create or Update Instructor Profile
        [HttpPost("{id}/profile")]
        public IActionResult CreateOrUpdateProfile(int id, InstructorProfile profile)
        {
            try
            {
                _service.CreateOrUpdateProfile(id, profile);
                return Ok("Profile created or updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 5 — Get Instructor Profile
        [HttpGet("{id}/profile")]
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
