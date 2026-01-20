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
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [JsonIgnore]
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
