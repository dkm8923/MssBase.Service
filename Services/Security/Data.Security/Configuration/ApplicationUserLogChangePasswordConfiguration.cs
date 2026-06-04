using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationUserLogChangePasswordConfiguration : IEntityTypeConfiguration<ApplicationUserLogChangePassword>
{
    private readonly string _tableName = "ApplicationUserLogChangePassword";

    public void Configure(EntityTypeBuilder<ApplicationUserLogChangePassword> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.OldPassword).IsRequired().HasMaxLength(256).IsUnicode(true);
        builder.Property(t => t.CreatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }

    public void SetTableName(EntityTypeBuilder<ApplicationUserLogChangePassword> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUserLogChangePassword> builder)
    {
        builder.HasKey(e => e.ApplicationUserLogChangePasswordId);
    }

    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUserLogChangePassword> builder)
    {
        builder.HasIndex(e => new { e.ApplicationUserId, e.ApplicationId, e.OldPassword }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "ApplicationUserId_ApplicationId_OldPassword"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUserLogChangePassword> builder)
    {
        builder.HasOne(d => d.Application)
            .WithMany()
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.ApplicationUser)
            .WithMany()
            .HasForeignKey(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}
