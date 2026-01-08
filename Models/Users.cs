using System.ComponentModel.DataAnnotations;

namespace Platec.Models
{
    public enum UserRole
    {
        Admin,
        Teacher,
        Student
    }
    public class Users
    {
        [Key]
        public int Userid { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
