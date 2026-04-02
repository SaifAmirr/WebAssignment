using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IInstructorService
    {
        Task<List<InstructorResponseDto>> GetAllAsync();
        Task<InstructorResponseDto> GetByIdAsync(int id);
        Task AddAsync(Instructor instructor);
        Task UpdateAsync(Instructor instructor);
        Task CreateOrUpdateProfileAsync(int instructorId, InstructorProfile profile);
        Task UpdateProfileAsync(InstructorProfile profile);
        Task<InstructorProfileResponseDto> GetProfileAsync(int instructorId);
        Task<List<CourseResponseDto>> GetInstructorCoursesAsync(int instructorId);
    }
}
