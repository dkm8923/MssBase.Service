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

        DataUtilities.ConfigureAuditFields(builder);

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
        dataArr.Add(new ApplicationUserRole { ApplicationUserRoleId = 1, ApplicationId = 2, ApplicationUserId = 1, RoleId = 2, Active = true});
        
        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
