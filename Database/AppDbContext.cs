using API_Workshop.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Database
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<ModuleInstructor> ModuleInstructors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>().HasKey(e => new {e.StudentId, e.CourseId});
            modelBuilder.Entity<ModuleInstructor>().HasKey(e => new { e.ModuleId, e.InstructorId });
            modelBuilder.Entity<Module>().HasOne(m => m.Course)
                .WithMany();
        }
    }

}
