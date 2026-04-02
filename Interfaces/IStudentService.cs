using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto> GetByIdAsync(int id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task EnrollStudentInCourseAsync(int studentId, int courseId);
        Task<List<CourseResponseDto>> GetStudentCoursesAsync(int studentId);
        Task WithdrawFromCourseAsync(int studentId, int courseId);
    }
}