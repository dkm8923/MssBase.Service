using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic;

namespace Data.Security.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly string _tableName = "User";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.UserId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.Email).HasMaxLength(128).IsRequired().IsUnicode(false);
        builder.Property(t => t.FirstName).HasMaxLength(64).IsUnicode(false);
        builder.Property(t => t.LastName).HasMaxLength(64).IsUnicode(false);
        builder.Property(t => t.DateOfBirth).HasPrecision(2);
        //builder.Property(t => t.ApplicationId).IsRequired();
        
        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        //CreateForeignKeys(builder);
        //CreateTableData(builder);
    }
        
    public void SetTableName(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.UserId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "Email"));
    }

    // public void CreateForeignKeys(EntityTypeBuilder<User> builder) 
    // {
    //     builder.HasOne(d => d.Application)
    //         .WithMany(p => p.ApplicationUsers)
    //         .HasForeignKey(d => d.ApplicationId)
    //         .OnDelete(DeleteBehavior.ClientSetNull)
    //         .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "Application"));
    // }
}

