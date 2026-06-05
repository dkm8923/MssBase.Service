using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationUserLogLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogLogin>
{
    private readonly string _tableName = "ApplicationUser_Log_Login";

    public void Configure(EntityTypeBuilder<ApplicationUserLogLogin> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.AuthToken).IsRequired().HasMaxLength(4096).IsUnicode(true);
        builder.Property(t => t.RefreshToken).IsRequired().HasMaxLength(2048).IsUnicode(true);
        builder.Property(t => t.CreatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }

    public void SetTableName(EntityTypeBuilder<ApplicationUserLogLogin> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUserLogLogin> builder)
    {
        builder.HasKey(e => e.LogId);
    }

    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUserLogLogin> builder)
    {
        builder.HasIndex(e => new { e.ApplicationUserId, e.ApplicationId, e.CreatedOn }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "ApplicationUserId_ApplicationId_CreatedOn"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUserLogLogin> builder)
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
