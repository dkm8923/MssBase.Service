using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using Data.Security.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Models;

public partial class SecurityDBContext : DbContext
{
    public SecurityDBContext(DbContextOptions<SecurityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ApplicationUserPermission> ApplicationUserPermissions { get; set; }
    public virtual DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        // Read connection string from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connectionString = config.GetSection("SecurityConnectionStrings:ReadWrite").Value;
        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserPermissionConfiguration());
    }
}
