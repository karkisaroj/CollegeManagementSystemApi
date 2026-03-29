using API_Workshop.Database;
using API_Workshop.Entities;
using API_Workshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Services.Implementations
{
    public class ModuleInstructorService(AppDbContext context) : IModuleInstructorService
    {
        private readonly AppDbContext _context = context;

        public async Task<ModuleInstructor?> GetByIdAsync(int moduleId, int instructorId)
        {
            return await _context.ModuleInstructors
                .FirstOrDefaultAsync(mi => mi.ModuleId == moduleId && mi.InstructorId == instructorId);
        }

        public async Task<ModuleInstructor> CreateAsync(ModuleInstructor moduleInstructor)
        {
            _context.ModuleInstructors.Add(moduleInstructor);
            await _context.SaveChangesAsync();
            return moduleInstructor;
        }

        public async Task<bool> DeleteAsync(int moduleId, int instructorId)
        {
            var assignment = await _context.ModuleInstructors
                .FirstOrDefaultAsync(mi => mi.ModuleId == moduleId && mi.InstructorId == instructorId);

            if (assignment == null) return false;

            _context.ModuleInstructors.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int moduleId, int instructorId)
        {
            return await _context.ModuleInstructors
                .AnyAsync(mi => mi.ModuleId == moduleId && mi.InstructorId == instructorId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.ModuleInstructors.CountAsync();
        }

        public async Task<IEnumerable<object>> GetFullDetailsAsync()
        {
            return await _context.ModuleInstructors
                .Include(mi => mi.Module)
                .Include(mi => mi.Instructor)
                .Select(mi => new
                {
                    mi.ModuleId,
                    ModuleTitle = mi.Module!.Title,
                    ModuleName = mi.Module.Name,
                    mi.InstructorId,
                    InstructorName = mi.Instructor!.FirstName + " " + mi.Instructor.LastName,
                    InstructorEmail = mi.Instructor.Email
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetInstructorModuleCountsAsync()
        {
            return await _context.ModuleInstructors
                .GroupBy(mi => mi.InstructorId)
                .Select(g => new
                {
                    InstructorId = g.Key,
                    ModuleCount = g.Count()
                })
                .ToListAsync();
        }

        public async Task<int> BulkCreateAsync(List<ModuleInstructor> assignments)
        {
            await _context.ModuleInstructors.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
            return assignments.Count;
        }
    }
}
