using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class UserLogLoginConfiguration : IEntityTypeConfiguration<UserLogLogin>
{
    private readonly string _tableName = "User_Log_Login";

    public void Configure(EntityTypeBuilder<UserLogLogin> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.AuthToken).IsRequired().HasMaxLength(4096).IsUnicode(true);
        builder.Property(t => t.RefreshToken).IsRequired().HasMaxLength(2048).IsUnicode(true);
        builder.ConfigureCreatedAuditFields();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }

    public void SetTableName(EntityTypeBuilder<UserLogLogin> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<UserLogLogin> builder)
    {
        builder.HasKey(e => e.LogId);
    }

    public void CreateUniqueKey(EntityTypeBuilder<UserLogLogin> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.ApplicationId, e.CreatedOn }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "UserId_ApplicationId_CreatedOn"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<UserLogLogin> builder)
    {
        builder.HasOne(d => d.Application)          
            .WithMany()
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "Application"));

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}
