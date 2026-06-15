using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.Common.Models;

public class CommonDBContextDesignTimeFactory : IDesignTimeDbContextFactory<CommonDBContext>
{
    public CommonDBContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetSection("CommonConnectionStrings:ReadWrite").Value;

        var optionsBuilder = new DbContextOptionsBuilder<CommonDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CommonDBContext(optionsBuilder.Options);
    }
}
