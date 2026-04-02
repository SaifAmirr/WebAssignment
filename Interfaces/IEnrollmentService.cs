using WebAssignment.Models;

namespace WebAssignment.Interfaces
{
    public interface IEnrollmentService
    {
        Enrollment GetEnrollment(int studentId, int courseId);
        void UpdateEnrollment(Enrollment enrollment);
        List<Enrollment> GetStudentEnrollments(int studentId);
        List<Enrollment> GetCourseEnrollments(int courseId);
    }
}
