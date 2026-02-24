using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Services
{
    public class StudentService : IStudentService
    {
        private static List<Student> students = new List<Student>()
        {
            new Student { Id = 1, Name = "Saif", GPA = 3.2 },
            new Student { Id = 2, Name = "Amir", GPA = 3.8 }
        };

        public List<Student> GetAll() => students;

        public Student GetById(int id)
        {
            return students.FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        public void Add(Student student)
        {
            students.Add(student);
        }
    }
}