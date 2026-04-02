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
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate,
                    StudentName = e.Student!.Name,
                    CourseName = e.Course!.Title
                })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Enrollment for student {studentId} in course {courseId} not found.");
        }

        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            // Validate that student exists
            var student = await _context.Students.FindAsync(enrollment.StudentId)
                ?? throw new KeyNotFoundException($"Student with Id {enrollment.StudentId} not found.");

            // Validate that course exists
            var course = await _context.Courses.FindAsync(enrollment.CourseId)
                ?? throw new KeyNotFoundException($"Course with Id {enrollment.CourseId} not found.");

            // Check if enrollment already exists
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);
            
            if (existingEnrollment != null)
                throw new InvalidOperationException($"Student {enrollment.StudentId} is already enrolled in course {enrollment.CourseId}.");

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
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
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate,
                    StudentName = e.Student!.Name,
                    CourseName = e.Course!.Title
                })
                .ToListAsync();
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseEnrollmentsAsync(int courseId)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    EnrollmentDate = e.EnrollmentDate,
                    StudentName = e.Student!.Name,
                    CourseName = e.Course!.Title
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
