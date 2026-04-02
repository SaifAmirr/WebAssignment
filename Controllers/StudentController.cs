using Microsoft.AspNetCore.Mvc;
using WebAssignment.DTOs;
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
            var response = students.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                GPA = s.GPA
            }).ToList();
            return Ok(response);
        }

        // Endpoint 2 — Get Student by Id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var student = _service.GetById(id);
                var response = new StudentResponseDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    GPA = student.GPA
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 3 — Add Student
        [HttpPost]
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
        public IActionResult Update(int id, [FromBody] StudentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var student = _service.GetById(id);
                student.Name = dto.Name;
                student.GPA = dto.GPA;

                _service.Update(student);

                var response = new StudentResponseDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    GPA = student.GPA
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

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

        [HttpGet("{studentId}/enrollments")]
        public IActionResult GetEnrollments(int studentId)
        {
            try
            {
                var enrollments = _service.GetStudentCourses(studentId);
                var student = _service.GetById(studentId);
                
                var enrollmentData = new List<dynamic>();
                foreach (var course in enrollments)
                {
                    var enrollment = new
                    {
                        StudentId = studentId,
                        StudentName = student.Name,
                        CourseId = course.Id,
                        CourseName = course.Title,
                        CreditHours = course.CreditHours,
                        InstructorName = course.Instructor?.Name
                    };
                    enrollmentData.Add(enrollment);
                }

                return Ok(enrollmentData);
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