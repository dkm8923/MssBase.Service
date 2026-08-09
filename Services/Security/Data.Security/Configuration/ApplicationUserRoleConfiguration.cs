using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    private readonly string _tableName = "ApplicationUserRole";
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserRoleId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.Property(t => t.RoleId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        //CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasKey(e => e.ApplicationUserRoleId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasIndex(e => new { e.ApplicationId, e.ApplicationUserId, e.RoleId }).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "ApplicationId_ApplicationUserId_RoleId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUserRole> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.ApplicationUserRoles)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.Role)
            .WithMany(p => p.ApplicationUserRoles)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Role"));

        builder.HasOne(d => d.ApplicationUser)
            .WithMany(p => p.ApplicationUserRoles)
            .HasForeignKey(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }

    public void CreateTableData(EntityTypeBuilder<ApplicationUserRole> builder) 
    {
        var dataArr = new List<ApplicationUserRole>();
        
        //Add All Admin Roles to Default Admin User
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 1}); //ApplicationAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 3}); //ApplicationUserAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 5}); //ApplicationUserPermissionAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 7}); //ApplicationUserRoleAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 9}); //PermissionAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 11}); //RoleAdmin
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 1, RoleId = 13}); //RolePermissionAdmin
        
        //Add All Readonly Roles to Default ReadOnly User
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 2}); //ApplicationReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 4}); //ApplicationUserReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 6}); //ApplicationUserPermissionReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 8}); //ApplicationUserRoleReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 10}); //PermissionReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 12}); //RoleReadOnly
        dataArr.Add(new ApplicationUserRole { ApplicationUserId = 2, RoleId = 14}); //RolePermissionReadOnly

        var defaultAppId = 1;
        var idx = 1;

        foreach (var applicationUserRole in dataArr)
        {
            applicationUserRole.ApplicationId = defaultAppId;
            applicationUserRole.ApplicationUserRoleId = idx++;
            applicationUserRole.Active = true;
            applicationUserRole.ReadOnly = true;
        }

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
