using WebAssignment.Interfaces;
using WebAssignment.Models;

namespace WebAssignment.Services
{
    public class CourseService : ICourseService
    {
        private static List<Course> courses = new List<Course>()
        {
            new Course { Id = 1, Title = "Math", CreditHours = 3 },
            new Course { Id = 2, Title = "Web Development", CreditHours = 4 }
        };

        public List<Course> GetAll() => courses;

        public Course GetById(int id)
            => courses.FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException($"Course with id {id} not found.");

        public void Add(Course course)
        {
            courses.Add(course);
        }
    }
}