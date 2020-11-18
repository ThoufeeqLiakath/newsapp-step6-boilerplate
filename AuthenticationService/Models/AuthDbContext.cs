using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Models
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {
            //make sure that database is auto generated using EF Core Code first
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // add your own configuration here
            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<User>().Property(x => x.UserId).ValueGeneratedNever();
            modelBuilder.Entity<User>().Property(x => x.UserId).IsRequired();            
            modelBuilder.Entity<User>().Property(x => x.UserId).IsRequired();
        }

        //Define a Dbset for User in the database
        public DbSet<User> Users { get; set; }
    }
}
