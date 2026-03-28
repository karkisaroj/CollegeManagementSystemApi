namespace API_Workshop.Entities
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Course Course { get; set; } = default!;
        public int CourseId { get; set; }

        public ICollection<ModuleInstructor> Instructors { get; set; } = [];
    }
}
