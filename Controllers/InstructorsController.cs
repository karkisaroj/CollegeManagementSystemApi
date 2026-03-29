using API_Workshop.Database;
using API_Workshop.DTO;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/instructors")]
    [ApiController]
    public class InstructorsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET /api/instructors - List all instructors
        [HttpGet]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await _context.Instructors.ToListAsync();
            return Ok(instructors);
        }

        // GET /api/instructors/{id} - Get instructor by Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null)
                return NotFound($"Instructor with ID {id} not found");
            return Ok(instructor);
        }

        // POST /api/instructors - Create instructor
        [HttpPost]
        public async Task<ActionResult<Instructor>> CreateInstructor([FromBody] InstructorCreateDto instructorDto)
        {
            var instructor = new Instructor
            {
                FirstName = instructorDto.FirstName,
                LastName = instructorDto.LastName,
                Email = instructorDto.Email,
                HireDate = instructorDto.HireDate
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstructorById), new { id = instructor.InstructorId }, instructor);
        }

        // PUT /api/instructors/{id} - Update instructor
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInstructor(int id, [FromBody] InstructorCreateDto instructorDto)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null)
                return NotFound($"Instructor with ID {id} not found");

            instructor.FirstName = instructorDto.FirstName;
            instructor.LastName = instructorDto.LastName;
            instructor.Email = instructorDto.Email;
            instructor.HireDate = instructorDto.HireDate;

            await _context.SaveChangesAsync();
            return Ok(instructor);
        }

        // DELETE /api/instructors/{id} - Delete instructor
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
            if (instructor == null)
                return NotFound($"Instructor with ID {id} not found");

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return Ok($"Instructor with ID {id} deleted successfully");
        }

        // POST /api/instructors/bulk - Bulk insert instructors
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertInstructors([FromBody] List<InstructorCreateDto> instructorsDto)
        {
            if (instructorsDto == null || instructorsDto.Count == 0)
                return BadRequest("Instructors data is required");

            var instructors = instructorsDto.Select(i => new Instructor
            {
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                HireDate = i.HireDate
            }).ToList();

            await _context.Instructors.AddRangeAsync(instructors);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{instructors.Count} instructors inserted successfully", Instructors = instructors });
        }

        // GET /api/instructors/modules - Get instructors with their modules
        [HttpGet("modules")]
        public async Task<IActionResult> GetInstructorsWithModules()
        {
            var instructors = await _context.Instructors
                .Include(i => i.ModuleInstructors)
                    .ThenInclude(mi => mi.Module)
                .ToListAsync();

            return Ok(instructors);
        }

        // GET /api/instructors/count - Total instructors
        [HttpGet("count")]
        public async Task<IActionResult> GetInstructorsCount()
        {
            var count = await _context.Instructors.CountAsync();
            return Ok(new { TotalInstructors = count });
        }

        // GET /api/instructors/distinct-hireyear - Unique hire years
        [HttpGet("distinct-hireyear")]
        public async Task<IActionResult> GetDistinctHireYears()
        {
            var hireYears = await _context.Instructors
                .Select(i => i.HireDate.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();

            return Ok(hireYears);
        }

        // GET /api/instructors/module-count - Number of modules per instructor
        [HttpGet("module-count")]
        public async Task<IActionResult> GetInstructorModuleCounts()
        {
            var instructorCounts = await _context.Instructors
                .Select(i => new
                {
                    i.InstructorId,
                    i.FirstName,
                    i.LastName,
                    ModuleCount = i.ModuleInstructors.Count
                })
                .ToListAsync();

            return Ok(instructorCounts);
        }
    }
}
