using API_Workshop.Entities;

namespace API_Workshop.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int studentId, int courseId);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(int studentId, int courseId);
        Task<bool> ExistsAsync(int studentId, int courseId);
        Task<int> GetCountAsync();
        Task<IEnumerable<Enrollment>> GetByDateRangeAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<object>> GetFullDetailsAsync();
    }
}
