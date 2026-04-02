
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

        public List<StudentResponseDto> GetAll() => _context.Students
            .AsNoTracking()
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                GPA = s.GPA
            })
            .ToList();

        public StudentResponseDto GetById(int id)
        {
            return _context.Students
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    GPA = s.GPA
                })
                .FirstOrDefault() ?? throw new KeyNotFoundException($"Student with id {id} not found.");
        }

        private Student GetStudentEntity(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"Student with id {id} not found.");
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
            var student = GetStudentEntity(studentId);
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId) ?? throw new KeyNotFoundException($"Course with id {courseId} not found.");

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

        public List<CourseResponseDto> GetStudentCourses(int studentId)
        {
            var student = GetStudentEntity(studentId);
            
            return _context.Enrollments
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
                .ToList();
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