
using WebAssignment.Interfaces;
using WebAssignment.Models;
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

        public List<Student> GetAll() => _context.Students.Include(s => s.Enrollments).ToList();

        public Student GetById(int id)
        {
            return _context.Students.Include(s => s.Enrollments).FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void EnrollStudentInCourse(int studentId, int courseId)
        {
            var student = GetById(studentId);
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId) ?? throw new KeyNotFoundException($"Course with id {courseId} not found.");

            // Check if already enrolled
            var existingEnrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
                throw new InvalidOperationException($"Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
        }

        public List<Course> GetStudentCourses(int studentId)
        {
            _ = GetById(studentId); // Verify student exists
            
            return _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .Where(c => c != null)
                .ToList()!;
        }

        public void WithdrawFromCourse(int studentId, int courseId)
        {
            var enrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new KeyNotFoundException($"Enrollment record not found for student {studentId} in course {courseId}.");

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
        }
    }
}