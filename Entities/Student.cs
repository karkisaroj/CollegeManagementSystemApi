using System.ComponentModel.DataAnnotations;

namespace API_Workshop.Entities
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; }= string.Empty;

        public DateOnly DateOfBirth { get; set; }
        public String Email { get; set; } = String.Empty;
        public String Phone { get; set; } = String.Empty;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
