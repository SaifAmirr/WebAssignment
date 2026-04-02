using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseResponseDto>> GetAllAsync();
        Task<CourseResponseDto> GetByIdAsync(int id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task<List<StudentResponseDto>> GetCourseEnrollmentsAsync(int courseId);
        Task AssignInstructorAsync(int courseId, int instructorId);
    }
}