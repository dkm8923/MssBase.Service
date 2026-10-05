using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Models;

namespace Shared.Data.Configuration;

/// <summary>
/// Shared mapping for note tables. Derive per parent entity (e.g. UserNote); CommonNote itself must not be mapped.
/// </summary>
public abstract class CommonNoteConfiguration<T> : IEntityTypeConfiguration<T> where T : CommonNote
{
    private readonly string _tableName;

    protected CommonNoteConfiguration(string tableName)
    {
        _tableName = tableName;
    }

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(e => e.NoteId);
        builder.Property(t => t.ReferenceId).IsRequired();
        builder.Property(t => t.NoteType).HasMaxLength(32).IsRequired();
        builder.Property(t => t.Subject).HasMaxLength(512).IsRequired();
        builder.Property(t => t.Text).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.ConfigureCreatedAuditFields();
    }
}
