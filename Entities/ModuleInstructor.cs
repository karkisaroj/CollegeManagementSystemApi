using System.ComponentModel.DataAnnotations;

namespace API_Workshop.Entities
{
    public class ModuleInstructor
    {
        public Module ?Module { get; set; }
        [Key]
        public int ModuleId {  get; set; }

        public Instructor ?Instructor { get; set; }
        
        [Key]
        public int InstructorId { get; set;}
    }
}
