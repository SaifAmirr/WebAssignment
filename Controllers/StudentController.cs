using Microsoft.AspNetCore.Mvc;
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        // Endpoint 1 — Get All Students
        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _service.GetAll();
            return Ok(students);
        }

        // Endpoint 2 — Get Student by Id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var student = _service.GetById(id);
                return Ok(student);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 3 — Add Student
        [HttpPost]
        public IActionResult Add(Student student)
        {
            _service.Add(student);
            return Ok(student);
        }

        // Endpoint 4 — Enroll Student in Course
        [HttpPost("{studentId}/enroll/{courseId}")]
        public IActionResult EnrollInCourse(int studentId, int courseId)
        {
            try
            {
                _service.EnrollStudentInCourse(studentId, courseId);
                return Ok("Student enrolled successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint 5 — Get All Courses for a Student
        [HttpGet("{id}/courses")]
        public IActionResult GetStudentCourses(int id)
        {
            try
            {
                var courses = _service.GetStudentCourses(id);
                return Ok(courses);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 6 — Withdraw from Course
        [HttpDelete("{studentId}/withdraw/{courseId}")]
        public IActionResult WithdrawFromCourse(int studentId, int courseId)
        {
            try
            {
                _service.WithdrawFromCourse(studentId, courseId);
                return Ok("Student withdrawn successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}