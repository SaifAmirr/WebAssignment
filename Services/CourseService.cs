using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> GetAll() => _context.Courses.ToList();

        public Course GetById(int id)
            => _context.Courses.FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }
    }
}