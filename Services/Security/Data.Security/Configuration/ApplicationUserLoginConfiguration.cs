using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic;

namespace Data.Security.Configuration;

public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
{
    private readonly string _tableName = "ApplicationUserLogin";

    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserLoginId).IsRequired();
        builder.Property(t => t.ApplicationUserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        
        builder.Property(t => t.Password).HasMaxLength(256).IsUnicode(true);
        builder.Property(t => t.PasswordResetRequired).IsRequired();
        builder.Property(t => t.LastLoginDate).HasPrecision(2);
        builder.Property(t => t.LastPasswordChangeDate).HasPrecision(2);
        builder.Property(t => t.LastLockoutDate).HasPrecision(2);
        builder.Property(t => t.FailedPasswordAttemptCount).HasDefaultValue((short)0);
        builder.Property(t => t.RefreshToken).HasMaxLength(2048).IsUnicode(false);
        builder.Property(t => t.RefreshTokenExpiryTime).HasPrecision(2);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }
        
    public void SetTableName(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        builder.HasKey(e => e.ApplicationUserLoginId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        builder.HasIndex(e => new { e.ApplicationUserId, e.ApplicationId }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "ApplicationUserId_ApplicationId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUserLogin> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.ApplicationUserLogins)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.ApplicationUser)
            .WithOne(p => p.ApplicationUserLogin)
            .HasForeignKey<ApplicationUserLogin>(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "ApplicationUser"));
    }
}

