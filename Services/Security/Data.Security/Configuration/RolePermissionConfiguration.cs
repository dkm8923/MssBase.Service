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

        DataUtilities.ConfigureAuditFields(builder);

        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.RoleId).IsRequired();
        builder.Property(t => t.PermissionId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        CreateTableData(builder); 
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
        dataArr.Add(new RolePermission { RolePermissionId = 1, ApplicationId = 2, RoleId = 1, PermissionId = 2, Active = true});
        
        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
