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
                var enrollmentDto = _service.GetEnrollment(studentId, courseId);
                var enrollment = new Enrollment
                {
                    Id = enrollmentDto.Id,
                    StudentId = studentId,
                    CourseId = courseId,
                    Grade = dto.Grade,
                    EnrollmentDate = enrollmentDto.EnrollmentDate
                };

                _service.UpdateEnrollment(enrollment);

                var response = _service.GetEnrollment(studentId, courseId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
