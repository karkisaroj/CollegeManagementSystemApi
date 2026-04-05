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
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    c.CourseId,
                    c.Name,
                    c.DurationYears,
                    ModuleCount = c.Modules.Count
                })
                .ToListAsync();
            return Ok(courses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound($"Course with ID {id} not found");

            return Ok(course);
        }

        [HttpGet("{id:int}/modules")]
        public async Task<IActionResult> GetModulesByCourseId(int id)
        {
            var modules = await _context.Modules
                .Where(m => m.CourseId == id)
                .Select(m => new
                {
                    m.ModuleId,
                    m.Title,
                    m.Name,
                    m.Credits,
                    m.CourseId
                })
                .ToListAsync();

            if (modules.Count == 0)
                return NotFound($"No modules found for course ID {id}");

            return Ok(modules);
        }

        [HttpGet("{id:int}/students")]
        public async Task<IActionResult> GetStudentsByCourseId(int id)
        {
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == id)
                .Select(e => new
                {
                    e.Student!.StudentId,
                    e.Student.FirstName,
                    e.Student.LastName,
                    e.Student.Email,
                    e.Student.Phone
                })
                .ToListAsync();

            if (students.Count == 0)
                return NotFound($"No students enrolled in course ID {id}");

            return Ok(students);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse([FromBody] CourseCreateDto courseDto)
        {
            var course = new Course
            {
                Name = courseDto.Name,
                DurationYears = courseDto.DurationYears
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, course);
        }

        [HttpPost("{id:int}/modules")]
        public async Task<ActionResult<Module>> AddModuleToCourse(int id, [FromBody] ModuleCreateDto moduleDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound($"Course with ID {id} not found");

            var module = new Module
            {
                Title = moduleDto.Title,
                Name = moduleDto.Name,
                Credits = moduleDto.Credits,
                CourseId = id
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return Ok(module);
        }

        // PUT /api/courses/{id} - Update course
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound($"Course with ID {id} not found");

            course.Name = courseDto.Name;
            course.DurationYears = courseDto.DurationYears;

            await _context.SaveChangesAsync();
            return Ok(course);
        }

        // DELETE /api/courses/{id} - Delete course (cascade handled by EF)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound($"Course with ID {id} not found");

            // Remove related modules and enrollments first
            _context.Modules.RemoveRange(course.Modules);
            _context.Enrollments.RemoveRange(course.Enrollments!);
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return Ok($"Course with ID {id} deleted successfully");
        }

        // POST /api/courses/bulk - Bulk insert courses
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertCourses([FromBody] List<CourseCreateDto> coursesDto)
        {
            if (coursesDto == null || coursesDto.Count == 0)
                return BadRequest("Courses data is required");

            var courses = coursesDto.Select(c => new Course
            {
                Name = c.Name,
                DurationYears = c.DurationYears
            }).ToList();

            await _context.Courses.AddRangeAsync(courses);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{courses.Count} courses inserted successfully", Courses = courses });
        }

        // GET /api/courses/with-details - Course + Modules + Instructors
        [HttpGet("with-details")]
        public async Task<IActionResult> GetCoursesWithDetails()
        {
            var courses = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Instructors)
                .Select(c => new
                {
                    c.CourseId,
                    c.Name,
                    c.DurationYears,
                    Modules = c.Modules.Select(m => new
                    {
                        m.ModuleId,
                        m.Title,
                        m.Name,
                        m.Credits,
                        Instructors = m.Instructors.Select(mi => new
                        {
                            mi!.Instructor!.InstructorId,
                            mi.Instructor.FirstName,
                            mi.Instructor.LastName,
                            mi.Instructor.Email
                        })
                    })
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET /api/courses/count - Total courses count
        [HttpGet("count")]
        public async Task<IActionResult> GetCoursesCount()
        {
            var count = await _context.Courses.CountAsync();
            return Ok(new { TotalCourses = count });
        }

        // GET /api/courses/total-credits - Sum of credits across all modules
        [HttpGet("total-credits")]
        public async Task<IActionResult> GetTotalCredits()
        {
            var totalCredits = await _context.Modules.SumAsync(m => m.Credits);
            return Ok(new { TotalCredits = totalCredits });
        }

        // GET /api/courses/top-enrolled - Courses with highest enrollments
        [HttpGet("top-enrolled")]
        public async Task<IActionResult> GetTopEnrolledCourses()
        {
            var topCourses = await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    CourseName = g.First().Course!.Name,
                    EnrollmentCount = g.Count()
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(5)
                .ToListAsync();

            return Ok(topCourses);
        }
    }
}