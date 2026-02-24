using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface ICourseService
    {
        List<Course> GetAll();
        Course GetById(int id);
        void Add(Course course);
    }
}