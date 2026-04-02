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

        public async Task<EnrollmentResponseDto> GetEnrollmentAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
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
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EnrollmentResponseDto>> GetStudentEnrollmentsAsync(int studentId)
        {
            return await _context.Enrollments
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
                .ToListAsync();
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseEnrollmentsAsync(int courseId)
        {
            return await _context.Enrollments
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
                .ToListAsync();
        }

        private async Task<Enrollment> GetEnrollmentEntityAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId)
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }
    }
}
