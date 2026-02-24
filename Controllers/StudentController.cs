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
            var student = _service.GetById(id);

            if (student == null)
                return NotFound("Student not found");

            return Ok(student);
        }

        // Endpoint 3 — Add Student
        [HttpPost]
        public IActionResult Add(Student student)
        {
            _service.Add(student);
            return Ok(student);
        }
    }
}