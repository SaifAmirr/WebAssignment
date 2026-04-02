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
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        // Endpoint 1 — Get All Students
        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult GetAll()
        {
            var students = _service.GetAll();
            return Ok(students);
        }

        // Endpoint 2 — Get Student by Id
        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Instructor,Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = new Student
            {
                Name = dto.Name,
                GPA = dto.GPA
            };

            _service.Add(student);
            
            var response = new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                GPA = student.GPA
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Admin")]
        public IActionResult Update(int id, [FromBody] StudentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var student = new Student
                {
                    Id = id,
                    Name = dto.Name,
                    GPA = dto.GPA
                };

                _service.Update(student);

                var updated = _service.GetById(id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{studentId}/enroll/{courseId}")]
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student,Instructor,Admin")]
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

        [HttpGet("{studentId}/enrollments")]
        [Authorize(Roles = "Student,Instructor,Admin")]
        public IActionResult GetEnrollments(int studentId)
        {
            try
            {
                var courses = _service.GetStudentCourses(studentId);
                var student = _service.GetById(studentId);
                
                var enrollmentData = courses.Select(c => new
                {
                    StudentId = studentId,
                    StudentName = student.Name,
                    CourseId = c.Id,
                    CourseName = c.Title,
                    CreditHours = c.CreditHours,
                    InstructorName = c.InstructorName
                }).ToList();
                
                return Ok(enrollmentData);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 6 — Withdraw from Course
        [HttpDelete("{studentId}/withdraw/{courseId}")]
        [Authorize(Roles = "Student")]
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