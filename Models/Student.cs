using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Platec.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }   // Optional

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int CourseId { get; set; }

        // Navigation Property
        [JsonIgnore]
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
