using API_Workshop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Workshop.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long>(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<ModuleInstructor> ModuleInstructors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser<long>>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("Roles");
            
            modelBuilder.Entity<Enrollment>().HasKey(e => new {e.StudentId, e.CourseId});
            modelBuilder.Entity<ModuleInstructor>().HasKey(e => new { e.ModuleId, e.InstructorId });
            modelBuilder.Entity<Module>().HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId);
        }
    }
}
