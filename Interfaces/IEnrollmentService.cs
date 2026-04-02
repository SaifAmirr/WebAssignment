using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto> GetEnrollmentAsync(int studentId, int courseId);
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task<List<EnrollmentResponseDto>> GetStudentEnrollmentsAsync(int studentId);
        Task<List<EnrollmentResponseDto>> GetCourseEnrollmentsAsync(int courseId);
    }
}
