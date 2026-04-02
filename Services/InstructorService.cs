using WebAssignment.Interfaces;
using WebAssignment.Models;
using WebAssignment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace WebAssignment.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly ApplicationDbContext _context;

        public InstructorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<InstructorResponseDto> GetAll() => _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Department = i.Department,
                Email = i.Email
            })
            .ToList();

        public InstructorResponseDto GetById(int id) => _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Department = i.Department,
                Email = i.Email
            })
            .FirstOrDefault() ?? throw new KeyNotFoundException($"Instructor with id {id} not found.");

        private Instructor GetInstructorEntity(int id) => _context.Instructors
            .FirstOrDefault(i => i.Id == id) ?? throw new KeyNotFoundException($"Instructor with id {id} not found.");

        public void Add(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
        }

        public void Update(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            _context.SaveChanges();
        }

        public void CreateOrUpdateProfile(int instructorId, InstructorProfile profile)
        {
            var instructor = GetInstructorEntity(instructorId);
            
            if (instructor.InstructorProfile == null)
            {
                profile.InstructorId = instructorId;
                _context.InstructorProfiles.Add(profile);
            }
            else
            {
                instructor.InstructorProfile.PhoneNumber = profile.PhoneNumber;
                instructor.InstructorProfile.OfficeLocation = profile.OfficeLocation;
                instructor.InstructorProfile.YearsOfExperience = profile.YearsOfExperience;
                _context.InstructorProfiles.Update(instructor.InstructorProfile);
            }

            _context.SaveChanges();
        }

        public void UpdateProfile(InstructorProfile profile)
        {
            _context.InstructorProfiles.Update(profile);
            _context.SaveChanges();
        }

        public InstructorProfileResponseDto GetProfile(int instructorId) => _context.InstructorProfiles
            .AsNoTracking()
            .Where(ip => ip.InstructorId == instructorId)
            .Select(ip => new InstructorProfileResponseDto
            {
                Id = ip.Id,
                PhoneNumber = ip.PhoneNumber,
                OfficeLocation = ip.OfficeLocation,
                YearsOfExperience = ip.YearsOfExperience,
                InstructorId = ip.InstructorId
            })
            .FirstOrDefault() ?? throw new KeyNotFoundException($"Profile for instructor {instructorId} not found.");

        public List<CourseResponseDto> GetInstructorCourses(int instructorId)
        {
            var instructor = GetInstructorEntity(instructorId);
            return _context.Courses
                .AsNoTracking()
                .Where(c => c.InstructorId == instructorId)
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
        }
    }
}
