using API_Workshop.Database;
using API_Workshop.DTO;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/moduleinstructors")]
    [ApiController]
    public class ModuleInstructorsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // POST /api/moduleinstructors - Assign instructor to module
        [HttpPost]
        public async Task<ActionResult<ModuleInstructor>> AssignInstructorToModule([FromBody] ModuleInstructorCreateDto dto)
        {
            var module = await _context.Modules.FindAsync(dto.ModuleId);
            if (module == null)
                return NotFound($"Module with ID {dto.ModuleId} not found");

            var instructor = await _context.Instructors.FindAsync(dto.InstructorId);
            if (instructor == null)
                return NotFound($"Instructor with ID {dto.InstructorId} not found");

            var existingAssignment = await _context.ModuleInstructors
                .FirstOrDefaultAsync(mi => mi.ModuleId == dto.ModuleId && mi.InstructorId == dto.InstructorId);
            if (existingAssignment != null)
                return BadRequest("Instructor is already assigned to this module");

            var moduleInstructor = new ModuleInstructor
            {
                ModuleId = dto.ModuleId,
                InstructorId = dto.InstructorId
            };

            _context.ModuleInstructors.Add(moduleInstructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModuleInstructorsCount), moduleInstructor);
        }

        // DELETE /api/moduleinstructors - Remove instructor from module
        [HttpDelete]
        public async Task<IActionResult> RemoveInstructorFromModule([FromQuery] int moduleId, [FromQuery] int instructorId)
        {
            var moduleInstructor = await _context.ModuleInstructors
                .FirstOrDefaultAsync(mi => mi.ModuleId == moduleId && mi.InstructorId == instructorId);

            if (moduleInstructor == null)
                return NotFound("Assignment not found");

            _context.ModuleInstructors.Remove(moduleInstructor);
            await _context.SaveChangesAsync();
            return Ok("Instructor removed from module successfully");
        }

        // POST /api/moduleinstructors/bulk - Bulk assignment
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkAssignInstructors([FromBody] List<ModuleInstructorCreateDto> assignmentsDto)
        {
            if (assignmentsDto == null || assignmentsDto.Count == 0)
                return BadRequest("Assignments data is required");

            var assignments = new List<ModuleInstructor>();
            foreach (var dto in assignmentsDto)
            {
                var module = await _context.Modules.FindAsync(dto.ModuleId);
                var instructor = await _context.Instructors.FindAsync(dto.InstructorId);

                if (module != null && instructor != null)
                {
                    var exists = await _context.ModuleInstructors
                        .AnyAsync(mi => mi.ModuleId == dto.ModuleId && mi.InstructorId == dto.InstructorId);
                    if (!exists)
                    {
                        assignments.Add(new ModuleInstructor
                        {
                            ModuleId = dto.ModuleId,
                            InstructorId = dto.InstructorId
                        });
                    }
                }
            }

            await _context.ModuleInstructors.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{assignments.Count} instructors assigned successfully" });
        }

        // GET /api/moduleinstructors/full-details - Module + Instructor details
        [HttpGet("full-details")]
        public async Task<IActionResult> GetFullDetails()
        {
            var assignments = await _context.ModuleInstructors
                .Include(mi => mi.Module)
                .Include(mi => mi.Instructor)
                .Select(mi => new
                {
                    mi.ModuleId,
                    ModuleTitle = mi.Module!.Title,
                    ModuleName = mi.Module.Name,
                    mi.InstructorId,
                    InstructorName = mi.Instructor!.FirstName + " " + mi.Instructor.LastName,
                    InstructorEmail = mi.Instructor.Email
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // GET /api/moduleinstructors/count - Total assignments
        [HttpGet("count")]
        public async Task<IActionResult> GetModuleInstructorsCount()
        {
            var count = await _context.ModuleInstructors.CountAsync();
            return Ok(new { TotalAssignments = count });
        }

        // GET /api/moduleinstructors/module-count - Instructor-wise module count
        [HttpGet("module-count")]
        public async Task<IActionResult> GetInstructorModuleCounts()
        {
            var counts = await _context.ModuleInstructors
                .GroupBy(mi => mi.InstructorId)
                .Select(g => new
                {
                    InstructorId = g.Key,
                    ModuleCount = g.Count()
                })
                .ToListAsync();

            return Ok(counts);
        }
    }
}
