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
        public async Task<IActionResult> GetAll()
        {
            var students = await _service.GetAllAsync();
            return Ok(students);
        }

        // Endpoint 2 — Get Student by Id
        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Instructor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var student = await _service.GetByIdAsync(id);
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
        public async Task<IActionResult> Add([FromBody] StudentCreateDto dto)
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

            await _service.AddAsync(student);
            
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
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
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

                await _service.UpdateAsync(student);

                var updated = await _service.GetByIdAsync(id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Endpoint 6 — Withdraw from Course
        [HttpDelete("{studentId}/withdraw/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> WithdrawFromCourse(int studentId, int courseId)
        {
            try
            {
                await _service.WithdrawFromCourseAsync(studentId, courseId);
                return Ok("Student withdrawn successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}