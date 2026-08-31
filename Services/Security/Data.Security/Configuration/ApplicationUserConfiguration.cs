using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic;

namespace Data.Security.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private readonly string _tableName = "ApplicationUser";

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        
        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }
        
    public void SetTableName(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(e => e.ApplicationUserId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.ApplicationId }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "UserId_ApplicationId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUser> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.ApplicationUsers)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.User)
            .WithMany(p => p.ApplicationUsers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}

