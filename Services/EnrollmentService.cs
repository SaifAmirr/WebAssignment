using WebAssignment.Interfaces;
using WebAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAssignment.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Enrollment GetEnrollment(int studentId, int courseId)
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId)
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }

        public void UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();
        }

        public List<Enrollment> GetStudentEnrollments(int studentId)
        {
            return _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToList();
        }

        public List<Enrollment> GetCourseEnrollments(int courseId)
        {
            return _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToList();
        }
    }
}
