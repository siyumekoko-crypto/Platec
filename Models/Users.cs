using System.ComponentModel.DataAnnotations;

namespace Platec.Models
{
    public enum UserRole
    {
        Admin = 1,
        Teacher = 2
    }
    public class User
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
