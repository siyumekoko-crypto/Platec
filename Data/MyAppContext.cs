using Microsoft.EntityFrameworkCore;
using Platec.Models;

namespace Platec.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ClassStatus> ClassStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClassStatus>()
                .HasOne(cs => cs.Student)
                .WithMany() // or .WithMany(s => s.ClassStatuses) if navigation exists
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // <-- NO CASCADE
        }

    }

}
