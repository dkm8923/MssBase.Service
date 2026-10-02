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
            CommonNoteId = source.CommonNoteId,
            NoteType = source.NoteType,
            Subject = source.Subject,
            Text = source.Text,
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

    // public static CommonNote ToEntityOnInsert(this InsertUpdateCommonNoteRequest source, string referenceType, int referenceId, string currentUser)
    // {
    //     if (source == null)
    //     {
    //         return null;
    //     }

    //     var target = new CommonNote
    //     {
    //         ReferenceType = referenceType,
    //         ReferenceId = referenceId,
    //         NoteType = source.NoteType,
    //         Subject = source.Subject,
    //         Text = source.Text,
    //         CurrentUser = currentUser
    //     };

    //     return target;
    // }

    public static CommonNote ToEntityOnInsert(this InsertUpdateCommonNoteRequest source, int referenceId, string currentUser)
    {
        if (source == null)
        {
            return null;
        }

        var target = new CommonNote
        {
            ReferenceId = referenceId,
            NoteType = source.NoteType,
            Subject = source.Subject,
            Text = source.Text,
            CurrentUser = currentUser
        };

        return target;
    }
}