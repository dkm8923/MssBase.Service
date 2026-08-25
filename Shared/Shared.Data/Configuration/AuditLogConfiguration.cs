using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Models;

namespace Shared.Data.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    private readonly string _tableName = "AuditLog";
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.AuditLogId).IsRequired();
        builder.Property(t => t.LogType).HasMaxLength(32).IsRequired();
        builder.Property(t => t.ReferenceType).HasMaxLength(128).IsRequired();
        builder.Property(t => t.ReferenceId).IsRequired();
        builder.Property(t => t.ChangeLogJson).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.Property(t => t.RecordStateBeforeChangeJson).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.ConfigureCreatedAuditFields();
        
        CreatePrimaryKey(builder);
    }

    public void SetTableName(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.AuditLogId);
    }
}
