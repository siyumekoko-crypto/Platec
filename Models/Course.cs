using System.ComponentModel.DataAnnotations;

namespace Platec.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CourseName { get; set; }

        // Optional: Teacher assigned to the course
        public int? TeacherId { get; set; }  // FK to Users table
        public virtual Users Teacher { get; set; }

        // ✅ Students enrolled in this course
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
