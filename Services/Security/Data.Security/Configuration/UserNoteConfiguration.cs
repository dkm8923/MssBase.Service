using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Data.Models;
using Shared.Logic;

namespace Data.Security.Configuration;

public class UserNoteConfiguration : IEntityTypeConfiguration<UserNote>
{
    private readonly string _tableName = "UserNote";

    public void Configure(EntityTypeBuilder<UserNote> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.CommonNoteId).IsRequired();
        builder.Property(t => t.ReferenceId).IsRequired();
        builder.Property(t => t.NoteType).HasMaxLength(32).IsRequired();
        builder.Property(t => t.Subject).HasMaxLength(512).IsRequired();
        builder.Property(t => t.Text).HasMaxLength(4096).IsUnicode(true).IsRequired();
        builder.ConfigureCreatedAuditFields();
        
        CreatePrimaryKey(builder);
        //CreateUniqueKey(builder);
        CreateForeignKeys(builder);
    }
        
    public void SetTableName(EntityTypeBuilder<UserNote> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<UserNote> builder)
    {
        builder.HasKey(e => e.CommonNoteId);
    }
    // public void CreateUniqueKey(EntityTypeBuilder<UserNote> builder)
    // {
    //     builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName(DataUtilities.CreateUniqueKey(_tableName, "Email"));
    // }

    public void CreateForeignKeys(EntityTypeBuilder<UserNote> builder) 
    {
        builder.HasOne(d => d.User)
            .WithMany(p => p.Notes)
            .HasForeignKey(d => d.ReferenceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(_tableName, "User"));
    }
}

