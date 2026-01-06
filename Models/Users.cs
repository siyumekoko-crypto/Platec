using System.ComponentModel.DataAnnotations;

namespace Platec.Models
{
    public class Users
    {
        [Key]
        public int Userid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
