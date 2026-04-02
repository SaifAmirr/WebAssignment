using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface ICourseService
    {
        List<CourseResponseDto> GetAll();
        CourseResponseDto GetById(int id);
        void Add(Course course);
        void Update(Course course);
        List<StudentResponseDto> GetCourseEnrollments(int courseId);
        void AssignInstructor(int courseId, int instructorId);
    }
}