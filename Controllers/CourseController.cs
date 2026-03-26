using API_Workshop.Database;
using API_Workshop.DTO;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        [HttpGet]
        public async Task<IActionResult> GetAllCourse()
        {
            var courses = await _context.Courses.ToListAsync();
            var modulecount = await _context.Modules.CountAsync();
            return Ok(new
            {
                course = courses,
                modulecounts = modulecount,
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var courses = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
            return courses != null ? Ok(courses) : NotFound();
        }
        [HttpGet("{id:int}/module")]
        public async Task<IActionResult> GetModuleByCourseId(int id)
        {
            var modules = await _context.Modules.Where(m => m.CourseId == id).Select(m => new
            {
                m.ModuleId,
                m.Title,
                m.Name,
                m.Credits,
                m.CourseId
            }).ToListAsync();
            return modules.Count != 0 ? Ok(modules) : NotFound();
        }

        [HttpGet("{id:int}/students")]
        public async Task<IActionResult> GetStudentByCourseId(int id)
        {
            var studentcourse = await _context.Enrollments.Include(e => e.Course).Where(e => e.StudentId == id).Select(
              e => new
              {
                  e.Student!.FirstName,
                  e.Student!.LastName,
                  e.Student!.Email,
                  e.Student!.Phone
              }).ToListAsync();
            return studentcourse!=null ? Ok(studentcourse) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Course>> AddCourse([FromBody] CourseCreateDto course)
        {
            var newcourses = new Course
            {
                Name = course.Name,
                DurationYears = course.DurationYears,
            };
            _context.Courses.Add(newcourses);
            await _context.SaveChangesAsync();
            return newcourses!=null ? Ok(newcourses) : NotFound();
        }

        [HttpPost("/{id:int}/modules")]
        public async Task<ActionResult<Module>> AddModuleByCourseId(int id,[FromBody] ModuleCreateDto module)
        {
            var existingcourseid=await _context.Courses.FirstOrDefaultAsync(e => e.CourseId==id);
            if (existingcourseid == null)
            {
                return NotFound("Module id is not found");
            }
            var newModule = new Module
            {
                Title = module.Title,
                Credits= module.Credits,
                Name= module.Name,
            };
            
            return newModule!=null ? Ok(newModule) : NotFound();
        }
    }
}
