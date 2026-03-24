namespace API_Workshop.Entities
{
    public class Module
    {
        public int ModuleId { get; set; }
        public String Title { get; set; } = String.Empty;
        public int Credits { get; set; }
        public Course Course { get; set; } = default!;
        public int CourseId {  get; set; }

        public string Name { get; set; } = string.Empty;
        
    }
}
