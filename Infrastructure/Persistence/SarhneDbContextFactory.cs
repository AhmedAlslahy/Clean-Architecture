using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class SarhneDbContextFactory : IDesignTimeDbContextFactory<SarhneDbContext>
{
    public SarhneDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SarhneDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=.;Database=SarhneDb;Trusted_Connection=True;TrustServerCertificate=True");

        return new SarhneDbContext(optionsBuilder.Options);
    }
}