using Microsoft.EntityFrameworkCore;

namespace Belt1.Models
{
    public class Belt1Context : DbContext
    {
        public DbSet<Users> users { get; set; } // always make users lowercase
        public DbSet<Activities> activities { get; set; } // always make users lowercase
        public DbSet<Participants> participants { get; set; } // always make users lowercase


        // base() calls the parent class' constructor passing the "options" parameter along
        public Belt1Context(DbContextOptions<Belt1Context> options) : base(options) { }
    }
}