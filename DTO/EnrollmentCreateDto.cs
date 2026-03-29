namespace API_Workshop.DTO
{
    public class EnrollmentCreateDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime? EnrolledDate { get; set; }
    }
}
