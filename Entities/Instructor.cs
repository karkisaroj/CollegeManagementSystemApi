namespace API_Workshop.Entities
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FirstName {  get; set; }=string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }
}
