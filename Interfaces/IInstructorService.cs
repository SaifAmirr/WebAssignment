using WebAssignment.Models;
using WebAssignment.DTOs;

namespace WebAssignment.Interfaces
{
    public interface IInstructorService
    {
        List<InstructorResponseDto> GetAll();
        InstructorResponseDto GetById(int id);
        void Add(Instructor instructor);
        void Update(Instructor instructor);
        void CreateOrUpdateProfile(int instructorId, InstructorProfile profile);
        void UpdateProfile(InstructorProfile profile);
        InstructorProfileResponseDto GetProfile(int instructorId);
        List<CourseResponseDto> GetInstructorCourses(int instructorId);
    }
}
