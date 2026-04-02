using WebAssignment.Interfaces;
using WebAssignment.Models;
using WebAssignment.DTOs;
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

        public EnrollmentResponseDto GetEnrollment(int studentId, int courseId)
        {
            return _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate
                })
                .FirstOrDefault()
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }

        public void UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();
        }

        public List<EnrollmentResponseDto> GetStudentEnrollments(int studentId)
        {
            return _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate
                })
                .ToList();
        }

        public List<EnrollmentResponseDto> GetCourseEnrollments(int courseId)
        {
            return _context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate
                })
                .ToList();
        }

        private Enrollment GetEnrollmentEntity(int studentId, int courseId)
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId)
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }
    }
}
