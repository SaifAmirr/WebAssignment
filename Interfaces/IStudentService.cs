using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IStudentService
    {
        List<Student> GetAll();
        Student GetById(int id);
        void Add(Student student);
    }
}