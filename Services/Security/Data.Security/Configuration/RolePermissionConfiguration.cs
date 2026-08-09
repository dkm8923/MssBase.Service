using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    private readonly string _tableName = "RolePermission";
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.RolePermissionId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.RoleId).IsRequired();
        builder.Property(t => t.PermissionId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        //CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(e => e.RolePermissionId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasIndex(e => new { e.ApplicationId, e.RoleId, e.PermissionId }).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "ApplicationId_RoleId_PermissionId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<RolePermission> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(d => d.PermissionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Permission"));

        builder.HasOne(d => d.Role)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Role"));
    }

    public void CreateTableData(EntityTypeBuilder<RolePermission> builder) 
    {
        var dataArr = new List<RolePermission>();
        
        //Default Application RolePermissions
        
        //Role = ApplicationAdmin
        dataArr.Add(new RolePermission { RoleId = 1, PermissionId = 1 }); //ApplicationRead
        dataArr.Add(new RolePermission { RoleId = 1, PermissionId = 2 }); //ApplicationInsert
        dataArr.Add(new RolePermission { RoleId = 1, PermissionId = 3 }); //ApplicationUpdate
        dataArr.Add(new RolePermission { RoleId = 1, PermissionId = 4 }); //ApplicationDelete

        //Role = ApplicationReadOnly
        dataArr.Add(new RolePermission { RoleId = 2, PermissionId = 1 }); //ApplicationRead
        
        //Default ApplicationUser RolePermissions
        
        //Role = ApplicationUserAdmin
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 5 }); //ApplicationUserRead
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 6 }); //ApplicationUserInsert
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 7 }); //ApplicationUserUpdate
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 8 }); //ApplicationUserDelete
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 9 }); //ApplicationUserResetPassword
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 10 }); //ApplicationUserChangePassword
        dataArr.Add(new RolePermission { RoleId = 3, PermissionId = 11 }); //ApplicationUserPasswordChangeHistoryRead

        //Role = ApplicationUserReadOnly
        dataArr.Add(new RolePermission { RoleId = 4, PermissionId = 5 }); //ApplicationUserRead
        
        //Default ApplicationUserPermission RolePermissions
        
        //Role = ApplicationUserPermissionAdmin
        dataArr.Add(new RolePermission { RoleId = 5, PermissionId = 12 }); //ApplicationRead
        dataArr.Add(new RolePermission { RoleId = 5, PermissionId = 13 }); //ApplicationInsert
        dataArr.Add(new RolePermission { RoleId = 5, PermissionId = 14 }); //ApplicationUpdate
        dataArr.Add(new RolePermission { RoleId = 5, PermissionId = 15 }); //ApplicationDelete

        //Role = ApplicationUserPermissionReadOnly
        dataArr.Add(new RolePermission { RoleId = 6, PermissionId = 12 }); //ApplicationRead

        //Default ApplicationUserRole RolePermissions
        
        //Role = ApplicationUserRoleAdmin
        dataArr.Add(new RolePermission { RoleId = 7, PermissionId = 16 }); //ApplicationRead
        dataArr.Add(new RolePermission { RoleId = 7, PermissionId = 17 }); //ApplicationInsert
        dataArr.Add(new RolePermission { RoleId = 7, PermissionId = 18 }); //ApplicationUpdate
        dataArr.Add(new RolePermission { RoleId = 7, PermissionId = 19 }); //ApplicationDelete

        //Role = ApplicationUserRoleReadOnly
        dataArr.Add(new RolePermission { RoleId = 8, PermissionId = 16 }); //ApplicationRead

        //Default Permission RolePermissions
        
        //Role = PermissionAdmin
        dataArr.Add(new RolePermission { RoleId = 9, PermissionId = 20 }); //PermissionRead
        dataArr.Add(new RolePermission { RoleId = 9, PermissionId = 21 }); //PermissionInsert
        dataArr.Add(new RolePermission { RoleId = 9, PermissionId = 22 }); //PermissionUpdate
        dataArr.Add(new RolePermission { RoleId = 9, PermissionId = 23 }); //PermissionDelete

        //Role = PermissionReadOnly
        dataArr.Add(new RolePermission { RoleId = 10, PermissionId = 20 }); //PermissionRead

        //Default Role RolePermissions
        
        //Role = RoleAdmin
        dataArr.Add(new RolePermission { RoleId = 11, PermissionId = 24 }); //RoleRead
        dataArr.Add(new RolePermission { RoleId = 11, PermissionId = 25 }); //RoleInsert
        dataArr.Add(new RolePermission { RoleId = 11, PermissionId = 26 }); //RoleUpdate
        dataArr.Add(new RolePermission { RoleId = 11, PermissionId = 27 }); //RoleDelete

        //Role = RoleReadOnly
        dataArr.Add(new RolePermission { RoleId = 12, PermissionId = 24 }); //RoleRead

        //Default RolePermission RolePermissions
        
        //Role = RolePermissionAdmin
        dataArr.Add(new RolePermission { RoleId = 13, PermissionId = 28 }); //RolePermissionRead
        dataArr.Add(new RolePermission { RoleId = 13, PermissionId = 29 }); //RolePermissionInsert
        dataArr.Add(new RolePermission { RoleId = 13, PermissionId = 30 }); //RolePermissionUpdate
        dataArr.Add(new RolePermission { RoleId = 13, PermissionId = 31 }); //RolePermissionDelete

        //Role = RolePermissionReadOnly
        dataArr.Add(new RolePermission { RoleId = 14, PermissionId = 28 }); //RolePermissionRead

        var defaultAppId = 1;
        var idx = 1;

        foreach (var role in dataArr)
        {
            role.ApplicationId = defaultAppId;
            role.RolePermissionId = idx++;
            role.Active = true;
            role.ReadOnly = true;
        }

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
