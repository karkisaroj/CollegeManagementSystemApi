using API_Workshop.Database;
using API_Workshop.Entities;
using API_Workshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Services.Implementations
{
    public class EnrollmentService(AppDbContext context) : IEnrollmentService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Enrollments.CountAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByDateRangeAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(e => e.EnrolledDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.EnrolledDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<object>> GetFullDetailsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new
                {
                    e.StudentId,
                    StudentName = e.Student!.FirstName + " " + e.Student.LastName,
                    e.CourseId,
                    CourseName = e.Course!.Name,
                    e.EnrolledDate
                })
                .ToListAsync();
        }
    }
}
