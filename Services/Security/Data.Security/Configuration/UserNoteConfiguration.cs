using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Data.Configuration;
using Shared.Data.Models;
using Shared.Logic;

namespace Data.Security.Configuration;

public class UserNoteConfiguration : CommonNoteConfiguration<UserNote>
{
    private const string TableName = "UserNote";

    public UserNoteConfiguration() : base(TableName)
    {
    }

    public override void Configure(EntityTypeBuilder<UserNote> builder)
    {
        base.Configure(builder);

        builder.HasOne(d => d.User)
            .WithMany(p => p.Notes)
            .HasForeignKey(d => d.ReferenceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(DataUtilities.CreateForeignKey(TableName, "User"));
    }
}

