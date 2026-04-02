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

        public List<CourseResponseDto> GetAll() => _context.Courses
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
            .ToList();

        public CourseResponseDto GetById(int id) => _context.Courses
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
            .FirstOrDefault() ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        private Course GetCourseEntity(int id) => _context.Courses
            .FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public List<StudentResponseDto> GetCourseEnrollments(int courseId)
        {
            var course = GetCourseEntity(courseId);
            
            return _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .AsNoTracking()
                .Select(e => new StudentResponseDto
                {
                    Id = e.Student!.Id,
                    Name = e.Student!.Name,
                    GPA = e.Student!.GPA
                })
                .ToList();
        }

        public void AssignInstructor(int courseId, int instructorId)
        {
            var course = GetCourseEntity(courseId);
            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId) 
                ?? throw new KeyNotFoundException($"Instructor with id {instructorId} not found.");

            course.InstructorId = instructorId;
            _context.Courses.Update(course);
            _context.SaveChanges();
        }
    }
}