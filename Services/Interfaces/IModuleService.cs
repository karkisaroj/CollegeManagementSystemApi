using API_Workshop.Entities;

namespace API_Workshop.Services.Interfaces
{
    public interface IModuleService
    {
        Task<IEnumerable<Module>> GetAllAsync();
        Task<Module?> GetByIdAsync(int id);
        Task<Module?> GetWithCourseAsync(int id);
        Task<IEnumerable<Module>> GetWithCoursesAsync();
        Task<IEnumerable<Module>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Module>> GetHighCreditModulesAsync(int minCredits);
        Task<IEnumerable<object>> GetInstructorsByModuleIdAsync(int moduleId);
        Task<Module> CreateAsync(Module module);
        Task<Module> UpdateAsync(Module module);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
        Task<int> BulkUpdateCreditsAsync(List<int> moduleIds);
    }
}
