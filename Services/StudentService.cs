
using WebAssignment.Interfaces;
using WebAssignment.Models;
using WebAssignment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace WebAssignment.Services
{
    public class StudentService : IStudentService
    {

        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync() => await _context.Students
            .AsNoTracking()
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                GPA = s.GPA
            })
            .ToListAsync();

        public async Task<StudentResponseDto> GetByIdAsync(int id)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    GPA = s.GPA
                })
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        private async Task<Student> GetStudentEntityAsync(int id)
        {
            return await _context.Students
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            var student = await GetStudentEntityAsync(studentId);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId) ?? throw new KeyNotFoundException($"Course with id {courseId} not found.");

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
                throw new InvalidOperationException($"Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CourseResponseDto>> GetStudentCoursesAsync(int studentId)
        {
            var student = await GetStudentEntityAsync(studentId);
            
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Instructor)
                .AsNoTracking()
                .Select(e => new CourseResponseDto
                {
                    Id = e.Course!.Id,
                    Title = e.Course!.Title,
                    CreditHours = e.Course!.CreditHours,
                    InstructorId = e.Course!.InstructorId,
                    InstructorName = e.Course!.Instructor!.Name
                })
                .ToListAsync();
        }

        public async Task WithdrawFromCourseAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new KeyNotFoundException($"Enrollment record not found for student {studentId} in course {courseId}.");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}