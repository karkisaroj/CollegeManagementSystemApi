using API_Workshop.Entities;

namespace API_Workshop.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<IEnumerable<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(int id);
        Task<Instructor> CreateAsync(Instructor instructor);
        Task<Instructor> UpdateAsync(Instructor instructor);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
        Task<IEnumerable<int>> GetDistinctHireYearsAsync();
        Task<IEnumerable<object>> GetInstructorModuleCountsAsync();
        Task<IEnumerable<Instructor>> GetWithModulesAsync();
    }
}
