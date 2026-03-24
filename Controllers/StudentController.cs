using API_Workshop.Database;
using API_Workshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace API_Workshop.Controllers
{
    public class StudentCreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    [Route("api/students")]
    [ApiController]
    public class StudentsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{id:int}")] 
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null) return NotFound($"Student with the Id {id} not found");
            return Ok(student);
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult> GetStudentByIdCourse(int id)
        {
            var courses = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == id)
                .Select(e => new
                {
                    e.CourseId,
                    e.Course!.Name
                })
                .ToListAsync();

            if (!courses.Any()) return NotFound("No courses found for this student");
            return Ok(courses);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> AddNewStudent([FromBody] StudentCreateDto newStudentDto)
        {
            var newStudent = new Student
            {
                FirstName = newStudentDto.FirstName,
                LastName = newStudentDto.LastName,
                DateOfBirth = newStudentDto.DateOfBirth,
                Email = newStudentDto.Email,
                Phone = newStudentDto.Phone
            };

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.StudentId }, newStudent);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Student>> UpdateStudent(int id, [FromBody] StudentCreateDto updatedStudentDto)
        {
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
            if (existingStudent == null)
            {
                return NotFound($"No student with the id {id}");
            }
            existingStudent.FirstName = updatedStudentDto.FirstName;
            existingStudent.LastName = updatedStudentDto.LastName;
            existingStudent.Email = updatedStudentDto.Email;
            existingStudent.Phone = updatedStudentDto.Phone;
            existingStudent.DateOfBirth = updatedStudentDto.DateOfBirth;
            
            await _context.SaveChangesAsync();
            return Ok(existingStudent);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteStudentsById(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s=>s.StudentId==id);
            if (student == null) return NotFound("Student with this id does not exist");
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok($"Deleted the student with Id {id}");
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert([FromBody] List<Student> students)
        {
            if (students == null || students.Count == 0) return BadRequest("Students data is required");
            await _context.Students.AddRangeAsync(students);
            await _context.SaveChangesAsync();
            return Ok("Students inserted successfully");
        }

        [HttpGet("with-courses")]
        public async Task<IActionResult> GetStudentsWithCourses()
        {
            var students = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .ToListAsync();
            return Ok(students);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _context.Students.CountAsync();
            return Ok(new { TotalStudents = count });
        }

        [HttpGet("full-details")]
        public async Task<IActionResult> GetFullDetails()
        {
            var students = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c!.Modules)
                .ToListAsync();
            return Ok(students);
        }
    }
}
