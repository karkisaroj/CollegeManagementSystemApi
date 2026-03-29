using API_Workshop.Database;
using API_Workshop.Entities;
using API_Workshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Services.Implementations
{
    public class CourseService(AppDbContext context) : ICourseService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course?> GetWithModulesAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<IEnumerable<Course>> GetWithDetailsAsync()
        {
            return await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Instructors)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopEnrolledAsync(int take = 5)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    CourseName = g.First().Course!.Name,
                    EnrollmentCount = g.Count()
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Course> CreateAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return false;

            _context.Modules.RemoveRange(course.Modules);
            _context.Enrollments.RemoveRange(course.Enrollments!);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Courses.CountAsync();
        }

        public async Task<int> GetTotalCreditsAsync()
        {
            return await _context.Modules.SumAsync(m => m.Credits);
        }

        public async Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId)
        {
            return await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    e.Student!.StudentId,
                    e.Student.FirstName,
                    e.Student.LastName,
                    e.Student.Email,
                    e.Student.Phone
                })
                .ToListAsync();
        }
    }
}
