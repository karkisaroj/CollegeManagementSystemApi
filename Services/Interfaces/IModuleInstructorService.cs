using API_Workshop.Entities;

namespace API_Workshop.Services.Interfaces
{
    public interface IModuleInstructorService
    {
        Task<ModuleInstructor?> GetByIdAsync(int moduleId, int instructorId);
        Task<ModuleInstructor> CreateAsync(ModuleInstructor moduleInstructor);
        Task<bool> DeleteAsync(int moduleId, int instructorId);
        Task<bool> ExistsAsync(int moduleId, int instructorId);
        Task<int> GetCountAsync();
        Task<IEnumerable<object>> GetFullDetailsAsync();
        Task<IEnumerable<object>> GetInstructorModuleCountsAsync();
        Task<int> BulkCreateAsync(List<ModuleInstructor> assignments);
    }
}
