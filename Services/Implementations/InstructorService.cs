using API_Workshop.Database;
using API_Workshop.Entities;
using API_Workshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Services.Implementations
{
    public class InstructorService(AppDbContext context) : IInstructorService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _context.Instructors.ToListAsync();
        }

        public async Task<Instructor?> GetByIdAsync(int id)
        {
            return await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
        }

        public async Task<Instructor> CreateAsync(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return instructor;
        }

        public async Task<Instructor> UpdateAsync(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            await _context.SaveChangesAsync();
            return instructor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null) return false;

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Instructors.AnyAsync(i => i.InstructorId == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Instructors.CountAsync();
        }

        public async Task<IEnumerable<int>> GetDistinctHireYearsAsync()
        {
            return await _context.Instructors
                .Select(i => i.HireDate.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetInstructorModuleCountsAsync()
        {
            return await _context.Instructors
                .Select(i => new
                {
                    i.InstructorId,
                    i.FirstName,
                    i.LastName,
                    ModuleCount = i.ModuleInstructors.Count
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Instructor>> GetWithModulesAsync()
        {
            return await _context.Instructors
                .Include(i => i.ModuleInstructors)
                    .ThenInclude(mi => mi.Module)
                .ToListAsync();
        }
    }
}
