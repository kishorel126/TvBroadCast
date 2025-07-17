using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TvBroadCast.DataAccess.DbContext
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {

        public AppDbContext CreateDbContext(string[] args)
        {
            // This method is used by EF Core tools to create a DbContext instance at design time.
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Configure the DbContext to use SQL Server with the specified connection string.
            optionsBuilder.UseSqlServer("Server=.;Database=TvBroadCast;Trusted_Connection=True;TrustServerCertificate=True");

            //Return a new instance of AppDbContext with the configured options.
            return new AppDbContext(optionsBuilder.Options);

        }

    }
}
