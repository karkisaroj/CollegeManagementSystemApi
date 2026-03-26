using System.ComponentModel.DataAnnotations;

namespace API_Workshop.DTO
{
    public class CourseCreateDto
    {
        public string Name { get; set; }=string.Empty;
        public int DurationYears { get; set; }=int.MaxValue;

    }
}
