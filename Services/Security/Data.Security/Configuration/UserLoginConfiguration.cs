using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic;

namespace Data.Security.Configuration;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    private readonly string _tableName = "UserLogin";

    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.UserLoginId).IsRequired();
        builder.Property(t => t.UserId).IsRequired();
        
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
        
    public void SetTableName(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<UserLogin> builder)
    {
        builder.HasKey(e => e.UserLoginId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<UserLogin> builder)
    {
        builder.HasIndex(e => new { e.UserId }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "UserId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<UserLogin> builder) 
    {
        builder.HasOne(d => d.User)
            .WithOne(p => p.UserLogin)
            .HasForeignKey<UserLogin>(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}

