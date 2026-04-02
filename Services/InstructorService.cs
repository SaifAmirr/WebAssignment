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

        public async Task<List<InstructorResponseDto>> GetAllAsync() => await _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Department = i.Department,
                Email = i.Email
            })
            .ToListAsync();

        public async Task<InstructorResponseDto> GetByIdAsync(int id) => await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Department = i.Department,
                Email = i.Email
            })
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Instructor with id {id} not found.");

        private async Task<Instructor> GetInstructorEntityAsync(int id) => await _context.Instructors
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Instructor with id {id} not found.");

        public async Task AddAsync(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task CreateOrUpdateProfileAsync(int instructorId, InstructorProfile profile)
        {
            var instructor = await GetInstructorEntityAsync(instructorId);
            
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

            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(InstructorProfile profile)
        {
            _context.InstructorProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<InstructorProfileResponseDto> GetProfileAsync(int instructorId) => await _context.InstructorProfiles
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
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Profile for instructor {instructorId} not found.");

        public async Task<List<CourseResponseDto>> GetInstructorCoursesAsync(int instructorId)
        {
            var instructor = await GetInstructorEntityAsync(instructorId);
            return await _context.Courses
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
                .ToListAsync();
        }
    }
}
