using WebAssignment.Interfaces;
using WebAssignment.Models;
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

        public List<Instructor> GetAll()
            => _context.Instructors.Include(i => i.InstructorProfile).Include(i => i.Courses).ToList();

        public Instructor GetById(int id)
            => _context.Instructors
                .Include(i => i.InstructorProfile)
                .Include(i => i.Courses)
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
            var instructor = GetById(instructorId);
            
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

        public InstructorProfile GetProfile(int instructorId)
        {
            var profile = _context.InstructorProfiles
                .FirstOrDefault(ip => ip.InstructorId == instructorId);
            
            return profile ?? throw new KeyNotFoundException($"Profile for instructor {instructorId} not found.");
        }

        public List<Course> GetInstructorCourses(int instructorId)
        {
            _ = GetById(instructorId); // Verify instructor exists
            return _context.Courses.Where(c => c.InstructorId == instructorId).ToList();
        }
    }
}
