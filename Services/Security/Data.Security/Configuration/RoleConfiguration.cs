using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private readonly string _tableName = "Role";
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.RoleId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);
        builder.Property(t => t.ApplicationId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        //CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.RoleId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "Name"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<Role> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.Roles)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "Application"));
    }

    public void CreateTableData(EntityTypeBuilder<Role> builder) 
    {
        var dataArr = new List<Role>();
        
        //Default Application Roles
        dataArr.Add(new Role { RoleId = 1, Name = "ApplicationAdmin", Description = "Full Access to all Application Functionality" });
        dataArr.Add(new Role { RoleId = 2, Name = "ApplicationReadOnly", Description = "ReadOnly Access to Application Functionality" });
        
        //Default ApplicationUser Roles
        dataArr.Add(new Role { RoleId = 3, Name = "ApplicationUserAdmin", Description = "Full Access to all ApplicationUser Functionality" });
        dataArr.Add(new Role { RoleId = 4, Name = "ApplicationUserReadOnly", Description = "ReadOnly Access to ApplicationUser Functionality" });

        //Default ApplicationUserPermission Roles
        dataArr.Add(new Role { RoleId = 5, Name = "ApplicationUserPermissionAdmin", Description = "Full Access to all ApplicationUserPermission Functionality" });
        dataArr.Add(new Role { RoleId = 6, Name = "ApplicationUserPermissionReadOnly", Description = "ReadOnly Access to ApplicationUserPermission Functionality" });

        //Default ApplicationUserRole Roles
        dataArr.Add(new Role { RoleId = 7, Name = "ApplicationUserRoleAdmin", Description = "Full Access to all ApplicationUserRole Functionality" });
        dataArr.Add(new Role { RoleId = 8, Name = "ApplicationUserRoleReadOnly", Description = "ReadOnly Access to ApplicationUserRole Functionality" });

        //Default Permission Roles
        dataArr.Add(new Role { RoleId = 9, Name = "PermissionAdmin", Description = "Full Access to all Permission Functionality" });
        dataArr.Add(new Role { RoleId = 10, Name = "PermissionReadOnly", Description = "ReadOnly Access to Permission Functionality" });

        //Default Role Roles
        dataArr.Add(new Role { RoleId = 11, Name = "RoleAdmin", Description = "Full Access to all Role Functionality" });
        dataArr.Add(new Role { RoleId = 12, Name = "RoleReadOnly", Description = "ReadOnly Access to Role Functionality" });

        //Default RolePermission Roles
        dataArr.Add(new Role { RoleId = 13, Name = "RolePermissionAdmin", Description = "Full Access to all RolePermission Functionality" });
        dataArr.Add(new Role { RoleId = 14, Name = "RolePermissionReadOnly", Description = "ReadOnly Access to RolePermission Functionality" });

        var defaultAppId = 1;
        
        foreach (var role in dataArr)
        {
            role.ApplicationId = defaultAppId;
            role.Active = true;
            role.ReadOnly = true;
        }

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
