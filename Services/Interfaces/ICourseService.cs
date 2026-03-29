using API_Workshop.Entities;

namespace API_Workshop.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course?> GetWithModulesAsync(int id);
        Task<IEnumerable<Course>> GetWithDetailsAsync();
        Task<IEnumerable<object>> GetTopEnrolledAsync(int take = 5);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
        Task<int> GetTotalCreditsAsync();
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId);
        Task<IEnumerable<object>> GetStudentsByCourseIdAsync(int courseId);
    }
}
