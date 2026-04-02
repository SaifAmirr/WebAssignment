using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebAssignment.DTOs;
using WebAssignment.Interfaces;
using WebAssignment.Models;


namespace WebAssignment.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var enrollment = new Enrollment
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _service.AddEnrollmentAsync(enrollment);

                var response = await _service.GetEnrollmentAsync(dto.StudentId, dto.CourseId);
                return Ok(response);
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

        [HttpGet("student/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentEnrollments(int studentId)
        {
            try
            {
                var enrollments = await _service.GetStudentEnrollmentsAsync(studentId);
                return Ok(enrollments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> GetCourseEnrollments(int courseId)
        {
            try
            {
                var enrollments = await _service.GetCourseEnrollmentsAsync(courseId);
                return Ok(enrollments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{studentId}/courses/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> UpdateGrade(int studentId, int courseId, [FromBody] EnrollmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var enrollmentDto = await _service.GetEnrollmentAsync(studentId, courseId);
                var enrollment = new Enrollment
                {
                    Id = enrollmentDto.Id,
                    StudentId = studentId,
                    CourseId = courseId,
                    Grade = dto.Grade,
                    EnrollmentDate = enrollmentDto.EnrollmentDate
                };

                await _service.UpdateEnrollmentAsync(enrollment);

                var response = await _service.GetEnrollmentAsync(studentId, courseId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
