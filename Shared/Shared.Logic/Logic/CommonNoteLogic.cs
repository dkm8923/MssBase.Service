using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Data.Converters;
using Shared.Models;
using Shared.Models.Dtos.CommonNote;

namespace Shared.Logic;

public class CommonNoteLogic //: ICommonNoteLogic
{
    public async Task<ErrorValidationResult<IEnumerable<CommonNoteDto>>> GetAllByReferenceId(int referenceId, BaseLogicGet req, ICommonNoteDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var query = dbContext.CommonNotes.AsQueryable().AsNoTracking().Where(cn => cn.ReferenceId == referenceId);

        query = query.ApplyIncludeInactiveFilter(req);
        query = query.ApplyIncludeReadOnlyFilter(req);

        return new ErrorValidationResult<IEnumerable<CommonNoteDto>> { Response = await query.ToDtos(cancellationToken) };
    }

    public async Task<ErrorValidationResult<IEnumerable<CommonNoteDto>>> GetNoteById(int referenceId, int noteId, BaseLogicGet req, ICommonNoteDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var query = dbContext.CommonNotes.AsQueryable().AsNoTracking().Where(cn => cn.ReferenceId == referenceId && cn.NoteId == noteId);

        query = query.ApplyIncludeInactiveFilter(req);
        query = query.ApplyIncludeReadOnlyFilter(req);

        return new ErrorValidationResult<IEnumerable<CommonNoteDto>> { Response = await query.ToDtos(cancellationToken) };
    }

    
    // public static async Task<List<CommonNoteDto>> GetAllCommonNotesByReferenceAsync(string referenceType, int referenceId, ICommonNoteDbContext dbContext)
    // {
    //     var commonNotes = await dbContext.CommonNotes
    //         .Where(n => n.ReferenceType == referenceType && n.ReferenceId == referenceId)
    //         .ToListAsync();

    //     return commonNotes.Select(n => n.ToDto()).ToList();
    // }
    
    // public static async Task InsertUpdateCommonNotes(List<InsertUpdateCommonNoteRequest>? commonNotes, 
    //                                                     string referenceType, 
    //                                                     int referenceId,
    //                                                     string currentUser, 
    //                                                     ICommonNoteDbContext dbContext
    //                                                 )
    // {
    //     if (commonNotes != null)
    //     {
    //         var commonNotesEntities = new List<CommonNote>();
    //         var existingNotes = await dbContext.CommonNotes
    //             .Where(n => n.ReferenceType == referenceType && n.ReferenceId == referenceId)
    //             .ToDictionaryAsync(n => n.CommonNoteId);

    //         foreach (var commonNote in commonNotes)
    //         {
    //             if (commonNote.CommonNoteId == null)
    //             {
    //                 commonNotesEntities.Add(commonNote.ToEntityOnInsert(referenceType, referenceId, currentUser));
    //             }
    //             else
    //             {
    //                 if (existingNotes.TryGetValue(commonNote.CommonNoteId.Value, out var existing))
    //                 {
    //                     existing.NoteType = commonNote.NoteType;
    //                     existing.Subject = commonNote.Subject;
    //                     existing.Text = commonNote.Text;
    //                     existing.CurrentUser = currentUser;
    //                 }
    //             }
    //         }
            
    //         await dbContext.CommonNotes.AddRangeAsync(commonNotesEntities);
    //     }
    // }

    // public static void DeleteAllCommonNotes(string referenceType, 
    //                                         int referenceId,
    //                                         string currentUser, 
    //                                         ICommonNoteDbContext dbContext
    //                                         )
    // {
    //     dbContext.CommonNotes.RemoveRange(dbContext.CommonNotes.Where(note => note.ReferenceType == referenceType && note.ReferenceId == referenceId));

    //     //Todo: Delete log for common note?
    // }
    
}