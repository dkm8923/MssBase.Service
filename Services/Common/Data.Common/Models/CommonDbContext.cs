using Microsoft.Extensions.Configuration;
//using Data.Common.Configuration;
using Microsoft.EntityFrameworkCore;
using Data.Common.Configuration;

namespace Data.Common.Models;

public partial class CommonDBContext : DbContext
{
    public CommonDBContext(DbContextOptions<CommonDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CommonRelationalData> CommonRelationalData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Read connection string from appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = config.GetSection("CommonConnectionStrings:ReadWrite").Value;
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommonRelationalDataConfiguration());
    }
}
