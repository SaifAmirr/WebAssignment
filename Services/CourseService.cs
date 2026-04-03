using WebAssignment.Interfaces;
using WebAssignment.Models;
using WebAssignment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace WebAssignment.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseResponseDto>> GetAllAsync() => await _context.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                CreditHours = c.CreditHours,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor!.Name
            })
            .ToListAsync();

        public async Task<CourseResponseDto> GetByIdAsync(int id) => await _context.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                CreditHours = c.CreditHours,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor!.Name
            })
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        private async Task<Course> GetCourseEntityAsync(int id) => await _context.Courses
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await GetCourseEntityAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StudentResponseDto>> GetCourseEnrollmentsAsync(int courseId)
        {
            var course = await GetCourseEntityAsync(courseId);
            
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .AsNoTracking()
                .Select(e => new StudentResponseDto
                {
                    Id = e.Student!.Id,
                    Name = e.Student!.Name,
                    GPA = e.Student!.GPA
                })
                .ToListAsync();
        }

        public async Task AssignInstructorAsync(int courseId, int instructorId)
        {
            var course = await GetCourseEntityAsync(courseId);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == instructorId) 
                ?? throw new KeyNotFoundException($"Instructor with id {instructorId} not found.");

            course.InstructorId = instructorId;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }
    }
}