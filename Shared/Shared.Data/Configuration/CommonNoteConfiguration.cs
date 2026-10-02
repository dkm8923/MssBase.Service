using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Models;

namespace Shared.Data.Configuration;

public class CommonNoteConfiguration : IEntityTypeConfiguration<CommonNote>
{
    private readonly string _tableName = "CommonNote";
    public void Configure(EntityTypeBuilder<CommonNote> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.CommonNoteId).IsRequired();
        builder.Property(t => t.ReferenceId).IsRequired();
        builder.Property(t => t.NoteType).HasMaxLength(32).IsRequired();
        builder.Property(t => t.Subject).HasMaxLength(512).IsRequired();
        builder.Property(t => t.Text).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.ConfigureCreatedAuditFields();
        
        CreatePrimaryKey(builder);
    }

    public void SetTableName(EntityTypeBuilder<CommonNote> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<CommonNote> builder)
    {
        builder.HasKey(e => e.CommonNoteId);
    }
}
