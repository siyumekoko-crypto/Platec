using Microsoft.EntityFrameworkCore;
using Platec.Models;

namespace Platec.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }
        public DbSet<Users> User { get; set; }
    }

}
