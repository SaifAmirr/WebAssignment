using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IInstructorService
    {
        List<Instructor> GetAll();
        Instructor GetById(int id);
        void Add(Instructor instructor);
        void CreateOrUpdateProfile(int instructorId, InstructorProfile profile);
        InstructorProfile GetProfile(int instructorId);
        List<Course> GetInstructorCourses(int instructorId);
    }
}
