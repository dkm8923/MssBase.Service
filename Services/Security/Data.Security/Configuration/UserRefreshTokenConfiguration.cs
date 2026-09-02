using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic;

namespace Data.Security.Configuration;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    private readonly string _tableName = "UserRefreshToken";

    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.UserRefreshTokenId).IsRequired();
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.RefreshToken).HasMaxLength(2048).IsUnicode(false);
        builder.Property(t => t.RefreshTokenExpiryTime).HasPrecision(2);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }
        
    public void SetTableName(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(e => e.UserRefreshTokenId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.ApplicationId }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "UserId_ApplicationId"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<UserRefreshToken> builder) 
    {
        builder.HasOne(d => d.User)
            .WithMany(p => p.UserRefreshTokens)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));

        builder.HasOne(d => d.Application)
            .WithMany(p => p.UserRefreshTokens)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));
    }
}

