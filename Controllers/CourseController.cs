using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CourseUpdateDto dto)
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

            try
            {
                var course = _service.GetById(id);
                course.Title = dto.Title;
                course.CreditHours = dto.CreditHours;
                course.InstructorId = dto.InstructorId;
                course.Instructor = instructor;

                _service.Update(course);

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

        // Endpoint 4 — Get All Enrolled Students for a Course
        [HttpGet("{id}/enrollments")]
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

        [HttpGet("{id}/enrollments/details")]
        public IActionResult GetEnrollmentDetails(int id)
        {
            try
            {
                var course = _service.GetById(id);
                var students = _service.GetCourseEnrollments(id);
                
                var enrollmentDetails = new List<dynamic>();
                foreach (var student in students)
                {
                    var detail = new
                    {
                        CourseId = id,
                        CourseName = course.Title,
                        StudentId = student.Id,
                        StudentName = student.Name,
                        StudentGPA = student.GPA
                    };
                    enrollmentDetails.Add(detail);
                }

                return Ok(enrollmentDetails);
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