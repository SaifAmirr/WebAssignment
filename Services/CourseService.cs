using WebAssignment.Interfaces;
using WebAssignment.Models;
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

        public List<Course> GetAll() => _context.Courses.Include(c => c.Instructor).Include(c => c.Enrollments).ToList();

        public Course GetById(int id)
            => _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public List<Student> GetCourseEnrollments(int courseId)
        {
            _ = GetById(courseId); // Verify course exists
            
            return _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Select(e => e.Student)
                .Where(s => s != null)
                .ToList()!;
        }

        public void AssignInstructor(int courseId, int instructorId)
        {
            var course = GetById(courseId);
            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId) 
                ?? throw new KeyNotFoundException($"Instructor with id {instructorId} not found.");

            course.InstructorId = instructorId;
            _context.Courses.Update(course);
            _context.SaveChanges();
        }
    }
}