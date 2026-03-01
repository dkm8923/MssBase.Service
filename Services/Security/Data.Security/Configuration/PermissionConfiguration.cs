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

        DataUtilities.ConfigureAuditFields(builder);

        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);
        builder.Property(t => t.ApplicationId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        CreateTableData(builder); 
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
        dataArr.Add(new Permission { PermissionId = 1, Name = "EosDefaultUser", Description = "Default Base Permission for EOS Application", Active = true, ApplicationId = 1 });
        
        dataArr.Add(new Permission { PermissionId = 2, Name = "EpcDefaultUser", Description = "Default Base Permission for EPC Application", Active = true, ApplicationId = 2 });
        dataArr.Add(new Permission { PermissionId = 3, Name = "CommissionReviewer", Description = "Default Base Permission for Commission Reviewer UI / Services", Active = true, ApplicationId = 2 });
        dataArr.Add(new Permission { PermissionId = 4, Name = "CommissionReviewerChangeContractor", Description = "Permission for Allowing Access to Change Contractor on Commission Reviewer UI / Services", Active = true, ApplicationId = 2 });
        dataArr.Add(new Permission { PermissionId = 5, Name = "CommissionReviewerAdjustRate", Description = "Permission for Adjusting Contractor Rates on Commission Reviewer UI / Services", Active = true, ApplicationId = 2 });

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}