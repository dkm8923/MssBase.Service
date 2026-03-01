using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationUserPermissionConfiguration : IEntityTypeConfiguration<ApplicationUserPermission>
{
    private readonly string _tableName = "ApplicationUserPermission";
    public void Configure(EntityTypeBuilder<ApplicationUserPermission> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserPermissionId).IsRequired();

        DataUtilities.ConfigureAuditFields(builder);

        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.Property(t => t.PermissionId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<ApplicationUserPermission> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUserPermission> builder)
    {
        builder.HasKey(e => e.ApplicationUserPermissionId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUserPermission> builder)
    {
        builder.HasIndex(e => new { e.ApplicationId, e.ApplicationUserId, e.PermissionId }).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "ApplicationId_ApplicationUserId_PermissionId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUserPermission> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.ApplicationUserPermissions)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.Permission)
            .WithMany(p => p.ApplicationUserPermissions)
            .HasForeignKey(d => d.PermissionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Permission"));

        builder.HasOne(d => d.ApplicationUser)
            .WithMany(p => p.ApplicationUserPermissions)
            .HasForeignKey(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }

    public void CreateTableData(EntityTypeBuilder<ApplicationUserPermission> builder) 
    {
        var dataArr = new List<ApplicationUserPermission>();
        dataArr.Add(new ApplicationUserPermission { ApplicationUserPermissionId = 1, ApplicationId = 2, ApplicationUserId = 1, PermissionId = 2, Active = true});
        
        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
