using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platec.Models
{
    public class ClassStatus
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Present, Absent, Late

        // Navigation Properties
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}
