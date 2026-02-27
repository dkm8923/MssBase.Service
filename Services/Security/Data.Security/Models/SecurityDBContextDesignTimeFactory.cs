using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.Security.Models;

public class SecurityDBContextDesignTimeFactory : IDesignTimeDbContextFactory<SecurityDBContext>
{
    public SecurityDBContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetSection("SecurityConnectionStrings:ReadWrite").Value;

        var optionsBuilder = new DbContextOptionsBuilder<SecurityDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SecurityDBContext(optionsBuilder.Options);
    }
}
    
