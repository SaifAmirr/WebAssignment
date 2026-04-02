using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IStudentService
    {
        List<StudentResponseDto> GetAll();
        StudentResponseDto GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void EnrollStudentInCourse(int studentId, int courseId);
        List<CourseResponseDto> GetStudentCourses(int studentId);
        void WithdrawFromCourse(int studentId, int courseId);
    }
}