using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IEnrollmentService
    {
        EnrollmentResponseDto GetEnrollment(int studentId, int courseId);
        void UpdateEnrollment(Enrollment enrollment);
        List<EnrollmentResponseDto> GetStudentEnrollments(int studentId);
        List<EnrollmentResponseDto> GetCourseEnrollments(int courseId);
    }
}
