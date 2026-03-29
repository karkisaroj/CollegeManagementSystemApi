using API_Workshop.Database;
using API_Workshop.Entities;
using API_Workshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Services.Implementations
{
    public class ModuleService(AppDbContext context) : IModuleService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Modules.ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            return await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<Module?> GetWithCourseAsync(int id)
        {
            return await _context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<IEnumerable<Module>> GetWithCoursesAsync()
        {
            return await _context.Modules
                .Include(m => m.Course)
                .Select(m => new Module
                {
                    ModuleId = m.ModuleId,
                    Title = m.Title,
                    Name = m.Name,
                    Credits = m.Credits,
                    CourseId = m.CourseId,
                    Course = m.Course
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Module>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Module>> GetHighCreditModulesAsync(int minCredits)
        {
            return await _context.Modules
                .Where(m => m.Credits > minCredits)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetInstructorsByModuleIdAsync(int moduleId)
        {
            return await _context.ModuleInstructors
                .Include(mi => mi.Instructor)
                .Where(mi => mi.ModuleId == moduleId)
                .Select(mi => new
                {
                    mi.Instructor!.InstructorId,
                    mi.Instructor.FirstName,
                    mi.Instructor.LastName,
                    mi.Instructor.Email
                })
                .ToListAsync();
        }

        public async Task<Module> CreateAsync(Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<Module> UpdateAsync(Module module)
        {
            _context.Modules.Update(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null) return false;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Modules.AnyAsync(m => m.ModuleId == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Modules.CountAsync();
        }

        public async Task<int> BulkUpdateCreditsAsync(List<int> moduleIds)
        {
            var modules = await _context.Modules
                .Where(m => moduleIds.Contains(m.ModuleId))
                .ToListAsync();

            foreach (var module in modules)
            {
                module.Credits += 1;
            }

            await _context.SaveChangesAsync();
            return modules.Count;
        }
    }
}
