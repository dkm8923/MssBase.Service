using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class UserLogChangePasswordConfiguration : IEntityTypeConfiguration<UserLogChangePassword>
{
    private readonly string _tableName = "User_Log_ChangePassword";

    public void Configure(EntityTypeBuilder<UserLogChangePassword> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.OldPassword).IsRequired().HasMaxLength(256).IsUnicode(true);
        builder.ConfigureCreatedAuditFields();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }

    public void SetTableName(EntityTypeBuilder<UserLogChangePassword> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<UserLogChangePassword> builder)
    {
        builder.HasKey(e => e.LogId);
    }

    public void CreateUniqueKey(EntityTypeBuilder<UserLogChangePassword> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.OldPassword }).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "UserId_OldPassword"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<UserLogChangePassword> builder)
    {
        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}
