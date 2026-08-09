using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    private readonly string _tableName = "Permission";
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.PermissionId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);
        builder.Property(t => t.ApplicationId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        //CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(e => e.PermissionId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<Permission> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "Name"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<Permission> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.Permissions)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "Application"));
    }

    public void CreateTableData(EntityTypeBuilder<Permission> builder) 
    {
        var dataArr = new List<Permission>();

        //Default Application Permissions
        dataArr.Add(new Permission { PermissionId = 1, Name = "ApplicationRead", Description = "Allows for retrieving all Application data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 2, Name = "ApplicationInsert", Description = "Allows for creating new Application data" });
        dataArr.Add(new Permission { PermissionId = 3, Name = "ApplicationUpdate", Description = "Allows for updating existing Application data" });
        dataArr.Add(new Permission { PermissionId = 4, Name = "ApplicationDelete", Description = "Allows for deleting Application data" });
        
        //Default ApplicationUser Permissions
        dataArr.Add(new Permission { PermissionId = 5, Name = "ApplicationUserRead", Description = "Allows for retrieving all ApplicationUser data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 6, Name = "ApplicationUserInsert", Description = "Allows for creating new ApplicationUser data" });
        dataArr.Add(new Permission { PermissionId = 7, Name = "ApplicationUserUpdate", Description = "Allows for updating existing ApplicationUser data" });
        dataArr.Add(new Permission { PermissionId = 8, Name = "ApplicationUserDelete", Description = "Allows for deleting ApplicationUser data" });
        dataArr.Add(new Permission { PermissionId = 9, Name = "ApplicationUserResetPassword", Description = "Allows for resetting an ApplicationUser's password" });
        dataArr.Add(new Permission { PermissionId = 10, Name = "ApplicationUserChangePassword", Description = "Allows for changing an ApplicationUser's password" });
        dataArr.Add(new Permission { PermissionId = 11, Name = "ApplicationUserPasswordChangeHistoryRead", Description = "Allows for retrieving an ApplicationUser's password change history in a read only state" });

        //Default ApplicationUserPermission Permissions
        dataArr.Add(new Permission { PermissionId = 12, Name = "ApplicationUserPermissionRead", Description = "Allows for retrieving all ApplicationUserPermission data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 13, Name = "ApplicationUserPermissionInsert", Description = "Allows for creating new ApplicationUserPermission data" });
        dataArr.Add(new Permission { PermissionId = 14, Name = "ApplicationUserPermissionUpdate", Description = "Allows for updating existing ApplicationUserPermission data" });
        dataArr.Add(new Permission { PermissionId = 15, Name = "ApplicationUserPermissionDelete", Description = "Allows for deleting ApplicationUserPermission data" });

        //Default ApplicationUserRole Permissions
        dataArr.Add(new Permission { PermissionId = 16, Name = "ApplicationUserRoleRead", Description = "Allows for retrieving all ApplicationUserRole data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 17, Name = "ApplicationUserRoleInsert", Description = "Allows for creating new ApplicationUserRole data" });
        dataArr.Add(new Permission { PermissionId = 18, Name = "ApplicationUserRoleUpdate", Description = "Allows for updating existing ApplicationUserRole data" });
        dataArr.Add(new Permission { PermissionId = 19, Name = "ApplicationUserRoleDelete", Description = "Allows for deleting ApplicationUserRole data" });

        //Default Permission Permissions
        dataArr.Add(new Permission { PermissionId = 20, Name = "PermissionRead", Description = "Allows for retrieving all Permission data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 21, Name = "PermissionInsert", Description = "Allows for creating new Permission data" });
        dataArr.Add(new Permission { PermissionId = 22, Name = "PermissionUpdate", Description = "Allows for updating existing Permission data" });
        dataArr.Add(new Permission { PermissionId = 23, Name = "PermissionDelete", Description = "Allows for deleting Permission data" });

        //Default Role Permissions
        dataArr.Add(new Permission { PermissionId = 24, Name = "RoleRead", Description = "Allows for retrieving all Role data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 25, Name = "RoleInsert", Description = "Allows for creating new Role data" });
        dataArr.Add(new Permission { PermissionId = 26, Name = "RoleUpdate", Description = "Allows for updating existing Role data" });
        dataArr.Add(new Permission { PermissionId = 27, Name = "RoleDelete", Description = "Allows for deleting Role data" });

        //Default RolePermission Permissions
        dataArr.Add(new Permission { PermissionId = 28, Name = "RolePermissionRead", Description = "Allows for retrieving all RolePermission data in a read only state" });
        dataArr.Add(new Permission { PermissionId = 29, Name = "RolePermissionInsert", Description = "Allows for creating new RolePermission data" });
        dataArr.Add(new Permission { PermissionId = 30, Name = "RolePermissionUpdate", Description = "Allows for updating existing RolePermission data" });
        dataArr.Add(new Permission { PermissionId = 31, Name = "RolePermissionDelete", Description = "Allows for deleting RolePermission data" });

        var defaultAppId = 1;
        
        foreach (var permission in dataArr)
        {
            permission.ApplicationId = defaultAppId;
            permission.Active = true;
            permission.ReadOnly = true;
        }

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}