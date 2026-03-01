using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private readonly string _tableName = "ApplicationUser";

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationUserId).IsRequired();

        DataUtilities.ConfigureAuditFields(builder);
        
        builder.Property(t => t.Email).HasMaxLength(128).IsRequired().IsUnicode(false);
        builder.Property(t => t.FirstName).HasMaxLength(64).IsUnicode(false);
        builder.Property(t => t.LastName).HasMaxLength(64).IsUnicode(false);
        builder.Property(t => t.DateOfBirth).HasPrecision(2);
        builder.Property(t => t.Password).HasMaxLength(64).IsUnicode(false);
        builder.Property(t => t.LastLoginDate).HasPrecision(2);
        builder.Property(t => t.LastPasswordChangeDate).HasPrecision(2);
        builder.Property(t => t.LastLockoutDate).HasPrecision(2);
        builder.Property(t => t.FailedPasswordAttemptCount).HasDefaultValue((short)0);
        builder.Property(t => t.ApplicationId).IsRequired();

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateForeignKeys(builder);
        CreateTableData(builder);
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
        builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "Email"));
    }

    public void CreateForeignKeys(EntityTypeBuilder<ApplicationUser> builder) 
    {
        builder.HasOne(d => d.Application)
            .WithMany(p => p.ApplicationUsers)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName( DataUtilities.CreateForeignKey(_tableName, "Application"));
    }

    public void CreateTableData(EntityTypeBuilder<ApplicationUser> builder) 
    {
        var dataArr = new List<ApplicationUser>();
        dataArr.Add(new ApplicationUser { 
             ApplicationUserId = 1
            ,Email = "dkm8923@gmail.com"
            ,FirstName = "Dan"
            ,LastName = "Mauk"
            ,ApplicationId = 2
            ,Active = true
        });

        dataArr.Add(new ApplicationUser { 
             ApplicationUserId = 2
            ,Email = "thompsonswartz@gmail.com"
            ,FirstName = "Rachel"
            ,LastName = "Thompson"
            ,ApplicationId = 1
            ,Active = true
        });

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}

