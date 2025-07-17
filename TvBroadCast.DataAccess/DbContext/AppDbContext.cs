using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TvBroadCast.Domain.Entities;


namespace TvBroadCast.DataAccess.DbContext
{
    public class AppDbContext : IdentityDbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //public DbSet<User> Users { get; set; } //IdentityUser

        public DbSet<BroadCast> BroadCasts { get; set; } // Broadcast entity

        public DbSet<ApprovalHistory> ApprovalHistories { get; set; } // Approval history entity


        // Override OnModelCreating to configure the model and relationships
        // DbSet properties for your entities
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


        }

    }
}
