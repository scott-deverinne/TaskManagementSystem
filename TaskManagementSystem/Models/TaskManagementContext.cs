using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Models
{
    public class TaskManagementContext : IdentityDbContext<AppUser>
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary keys for each model
            // Customize Identity model if needed
            modelBuilder.Entity<AppUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles").HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins").HasKey(ul => new { ul.ProviderKey });
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens").HasKey(ut => new { ut.UserId, ut.Value });
			modelBuilder.Entity<Ticket>().HasKey(ut => new { ut.Id });
           }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}