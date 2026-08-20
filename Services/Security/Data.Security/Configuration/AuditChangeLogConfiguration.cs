using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class AuditChangeLogConfiguration : IEntityTypeConfiguration<AuditChangeLog>
{
    private readonly string _tableName = "AuditChangeLog";
    public void Configure(EntityTypeBuilder<AuditChangeLog> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.AuditChangeLogId).IsRequired();
        builder.Property(t => t.ChangeType).HasMaxLength(32).IsRequired();
        builder.Property(t => t.ReferenceType).HasMaxLength(128).IsRequired();
        builder.Property(t => t.ReferenceId).IsRequired();
        builder.Property(t => t.Json).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.ConfigureCreatedAuditFields();
        
        CreatePrimaryKey(builder);
    }

    public void SetTableName(EntityTypeBuilder<AuditChangeLog> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<AuditChangeLog> builder)
    {
        builder.HasKey(e => e.AuditChangeLogId);
    }
}