using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Workshop.Entities
{
    public class Enrollment
    {
        [Key]
        public int StudentId { get; set; }
        [JsonIgnore]
        public Student? Student { get; set; }

        [Key]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public DateTime? EnrolledDate { get; set; }
    }
}
