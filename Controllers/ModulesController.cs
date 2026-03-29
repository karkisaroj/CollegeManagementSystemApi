using API_Workshop.Database;
using API_Workshop.DTO;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/modules")]
    [ApiController]
    public class ModulesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET /api/modules - List all modules
        [HttpGet]
        public async Task<IActionResult> GetAllModules()
        {
            var modules = await _context.Modules.ToListAsync();
            return Ok(modules);
        }

        // GET /api/modules/{id} - Get module by Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetModuleById(int id)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
                return NotFound($"Module with ID {id} not found");
            return Ok(module);
        }

        // GET /api/modules/{id}/instructors - Get instructors assigned to module
        [HttpGet("{id:int}/instructors")]
        public async Task<IActionResult> GetInstructorsByModuleId(int id)
        {
            var instructors = await _context.ModuleInstructors
                .Include(mi => mi.Instructor)
                .Where(mi => mi.ModuleId == id)
                .Select(mi => new
                {
                    mi.Instructor!.InstructorId,
                    mi.Instructor.FirstName,
                    mi.Instructor.LastName,
                    mi.Instructor.Email
                })
                .ToListAsync();

            if (instructors.Count == 0)
                return NotFound($"No instructors found for module ID {id}");

            return Ok(instructors);
        }

        // POST /api/modules - Create module with CourseId
        [HttpPost]
        public async Task<ActionResult<Module>> CreateModule([FromBody] ModuleCreateDto moduleDto)
        {
            var module = new Module
            {
                Title = moduleDto.Title,
                Name = moduleDto.Name,
                Credits = moduleDto.Credits,
                CourseId = moduleDto.CourseId
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModuleById), new { id = module.ModuleId }, module);
        }

        // PUT /api/modules/{id} - Update module
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] ModuleCreateDto moduleDto)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
                return NotFound($"Module with ID {id} not found");

            module.Title = moduleDto.Title;
            module.Name = moduleDto.Name;
            module.Credits = moduleDto.Credits;
            module.CourseId = moduleDto.CourseId;

            await _context.SaveChangesAsync();
            return Ok(module);
        }

        // DELETE /api/modules/{id} - Delete module
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
                return NotFound($"Module with ID {id} not found");

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return Ok($"Module with ID {id} deleted successfully");
        }

        // POST /api/modules/bulk - Bulk insert modules
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertModules([FromBody] List<ModuleCreateDto> modulesDto)
        {
            if (modulesDto == null || modulesDto.Count == 0)
                return BadRequest("Modules data is required");

            var modules = modulesDto.Select(m => new Module
            {
                Title = m.Title,
                Name = m.Name,
                Credits = m.Credits,
                CourseId = m.CourseId
            }).ToList();

            await _context.Modules.AddRangeAsync(modules);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{modules.Count} modules inserted successfully", Modules = modules });
        }

        // GET /api/modules/with-course - Get modules with course (Navigation Select)
        [HttpGet("with-course")]
        public async Task<IActionResult> GetModulesWithCourse()
        {
            var modules = await _context.Modules
                .Include(m => m.Course)
                .Select(m => new
                {
                    m.ModuleId,
                    m.Title,
                    m.Name,
                    m.Credits,
                    Course = new
                    {
                        m.Course.CourseId,
                        m.Course.Name
                    }
                })
                .ToListAsync();

            return Ok(modules);
        }

        // GET /api/modules/count - Total modules
        [HttpGet("count")]
        public async Task<IActionResult> GetModulesCount()
        {
            var count = await _context.Modules.CountAsync();
            return Ok(new { TotalModules = count });
        }

        // GET /api/modules/high-credit - Modules with credits > X
        [HttpGet("high-credit")]
        public async Task<IActionResult> GetHighCreditModules([FromQuery] int minCredits = 10)
        {
            var modules = await _context.Modules
                .Where(m => m.Credits > minCredits)
                .ToListAsync();

            return Ok(modules);
        }

        // PUT /api/modules/bulk-update-credits - Update multiple modules' credits
        [HttpPut("bulk-update-credits")]
        public async Task<IActionResult> BulkUpdateCredits([FromBody] List<int> moduleIds)
        {
            if (moduleIds == null || moduleIds.Count == 0)
                return BadRequest("Module IDs are required");

            var modules = await _context.Modules
                .Where(m => moduleIds.Contains(m.ModuleId))
                .ToListAsync();

            foreach (var module in modules)
            {
                module.Credits += 1; // Increment credits by 1
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{modules.Count} modules updated successfully" });
        }
    }
}
