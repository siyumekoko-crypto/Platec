using System.ComponentModel.DataAnnotations;

namespace Platec.Models
{
    public enum UserRole
    {
        Admin,
        Teacher
    }
    public class Users
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
