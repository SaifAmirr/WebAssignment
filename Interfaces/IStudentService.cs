using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IStudentService
    {
        List<Student> GetAll();
        Student GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void EnrollStudentInCourse(int studentId, int courseId);
        List<Course> GetStudentCourses(int studentId);
        void WithdrawFromCourse(int studentId, int courseId);
    }
}