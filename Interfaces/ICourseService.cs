using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface ICourseService
    {
        List<Course> GetAll();
        Course GetById(int id);
        void Add(Course course);
        void Update(Course course);
        List<Student> GetCourseEnrollments(int courseId);
        void AssignInstructor(int courseId, int instructorId);
    }
}