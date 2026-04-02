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

        [HttpPut("{studentId}/courses/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult UpdateGrade(int studentId, int courseId, [FromBody] EnrollmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var enrollment = _service.GetEnrollment(studentId, courseId);
                enrollment.Grade = dto.Grade;

                _service.UpdateEnrollment(enrollment);

                var response = new EnrollmentResponseDto
                {
                    Id = enrollment.Id,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    Grade = enrollment.Grade,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    StudentName = enrollment.Student?.Name,
                    CourseName = enrollment.Course?.Title
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
