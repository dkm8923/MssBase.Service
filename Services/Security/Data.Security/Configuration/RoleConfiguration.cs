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

        DataUtilities.ConfigureAuditFields(builder);

        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);
        builder.Property(t => t.ApplicationId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        CreateTableData(builder); 
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
        dataArr.Add(new Role { RoleId = 1, Name = "DataAnalyst", Description = "Super User Role for EPC Application", Active = true, ApplicationId = 2 });
        dataArr.Add(new Role { RoleId = 2, Name = "OfficeUser", Description = "Read Only Role for EPC Application", Active = true, ApplicationId = 2 });
        
        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}
