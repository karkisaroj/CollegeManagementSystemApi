using System.ComponentModel.DataAnnotations;

namespace API_Workshop.Entities
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationYears { get; set; }
        public List<Enrollment> ? Enrollments { get; set; }
        public ICollection<Module> Modules { get; set; } = [];
    }
}
