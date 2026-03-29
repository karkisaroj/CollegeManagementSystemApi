using API_Workshop.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = new
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalModules = await _context.Modules.CountAsync(),
                TotalEnrollments = await _context.Enrollments.CountAsync()
            };

            return Ok(summary);
        }
    }
}
