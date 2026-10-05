using Shared.Data.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Dtos.CommonNote;

namespace Shared.Data.Converters;

public static class CommonNoteConverters
{
    public static CommonNoteDto ToDto(this CommonNote source)
    {
        if (source == null)
        {
            return null;
        }

        var target = new CommonNoteDto
        {
            NoteId = source.NoteId,
            NoteType = source.NoteType,
            Subject = source.Subject,
            Text = source.Text,
            Active = source.Active,
            CreatedOn = source.CreatedOn,
            CreatedBy = source.CreatedBy,
            UpdatedOn = source.UpdatedOn,
            UpdatedBy = source.UpdatedBy
        };

        return target;
    }

    public static async Task<List<CommonNoteDto>> ToDtos(this IQueryable<CommonNote> source, CancellationToken cancellationToken = default)
    {
        if (source.Count() == 0)
        {
            return null;
        }

        var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

        return target;
    }

    public static T ToEntityOnInsert<T>(this InsertUpdateCommonNoteRequest source, int referenceId, string currentUser)
        where T : CommonNote, new()
    {
        if (source == null)
        {
            return null;
        }

        var target = new T
        {
            ReferenceId = referenceId,
            NoteType = source.NoteType,
            Subject = source.Subject,
            Text = source.Text,
            Active = source.Active,
            CurrentUser = currentUser
        };

        return target;
    }

    public static CommonNote UpdateEntityFromRequest(this CommonNote entity, InsertUpdateCommonNoteRequest source)
    {
        if (source == null || entity == null)
        {
            return null;
        }

        entity.Active = source.Active;
        entity.NoteType = source.NoteType;
        entity.Subject = source.Subject;
        entity.Text = source.Text;
        entity.CurrentUser = source.CurrentUser;

        return entity;
    }
}