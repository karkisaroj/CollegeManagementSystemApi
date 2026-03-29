using API_Workshop.Database;
using API_Workshop.DTO;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET /api/enrollments - List all enrollments
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            return Ok(enrollments);
        }

        // POST /api/enrollments - Enroll student in course
        [HttpPost]
        public async Task<ActionResult<Enrollment>> CreateEnrollment([FromBody] EnrollmentCreateDto enrollmentDto)
        {
            var student = await _context.Students.FindAsync(enrollmentDto.StudentId);
            if (student == null)
                return NotFound($"Student with ID {enrollmentDto.StudentId} not found");

            var course = await _context.Courses.FindAsync(enrollmentDto.CourseId);
            if (course == null)
                return NotFound($"Course with ID {enrollmentDto.CourseId} not found");

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == enrollmentDto.StudentId && e.CourseId == enrollmentDto.CourseId);
            if (existingEnrollment != null)
                return BadRequest("Student is already enrolled in this course");

            var enrollment = new Enrollment
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId,
                EnrolledDate = enrollmentDto.EnrolledDate ?? DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllEnrollments), enrollment);
        }

        // DELETE /api/enrollments - Remove enrollment (using query params for composite key)
        [HttpDelete]
        public async Task<IActionResult> DeleteEnrollment([FromQuery] int studentId, [FromQuery] int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound("Enrollment not found");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return Ok("Enrollment deleted successfully");
        }

        // POST /api/enrollments/bulk - Bulk enroll students
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkEnrollStudents([FromBody] List<EnrollmentCreateDto> enrollmentsDto)
        {
            if (enrollmentsDto == null || enrollmentsDto.Count == 0)
                return BadRequest("Enrollments data is required");

            var enrollments = new List<Enrollment>();
            foreach (var dto in enrollmentsDto)
            {
                var student = await _context.Students.FindAsync(dto.StudentId);
                var course = await _context.Courses.FindAsync(dto.CourseId);

                if (student != null && course != null)
                {
                    enrollments.Add(new Enrollment
                    {
                        StudentId = dto.StudentId,
                        CourseId = dto.CourseId,
                        EnrolledDate = dto.EnrolledDate ?? DateTime.Now
                    });
                }
            }

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{enrollments.Count} students enrolled successfully" });
        }

        // GET /api/enrollments/full-details - Student + Course + EnrollmentDate
        [HttpGet("full-details")]
        public async Task<IActionResult> GetFullDetails()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new
                {
                    e.StudentId,
                    StudentName = e.Student!.FirstName + " " + e.Student.LastName,
                    e.CourseId,
                    CourseName = e.Course!.Name,
                    e.EnrolledDate
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET /api/enrollments/count - Total enrollments
        [HttpGet("count")]
        public async Task<IActionResult> GetEnrollmentsCount()
        {
            var count = await _context.Enrollments.CountAsync();
            return Ok(new { TotalEnrollments = count });
        }

        // GET /api/enrollments/by-date - Filter enrollments by date
        [HttpGet("by-date")]
        public async Task<IActionResult> GetEnrollmentsByDate([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(e => e.EnrolledDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.EnrolledDate <= endDate.Value);
            }

            var enrollments = await query.ToListAsync();
            return Ok(enrollments);
        }
    }
}
