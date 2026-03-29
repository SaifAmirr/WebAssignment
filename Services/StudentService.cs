
using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Services
{
    public class StudentService : IStudentService
    {

        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAll() => _context.Students.ToList();

        public Student GetById(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }
    }
}