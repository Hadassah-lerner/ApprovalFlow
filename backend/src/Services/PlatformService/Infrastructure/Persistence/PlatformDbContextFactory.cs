using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlatformService.Infrastructure.Persistence
{ 
public class PlatformDbContextFactory
    : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<PlatformDbContext>();

        const string connectionString =
            "Host=localhost;Port=5432;Database=platform_db;Username=postgres;Password=password";

        optionsBuilder.UseNpgsql(connectionString);

        return new PlatformDbContext(optionsBuilder.Options);
    }
}
}