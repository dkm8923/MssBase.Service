using Shared.Data.Models;
using Shared.Models;
using Shared.Models.Contracts;
using Microsoft.AspNetCore.Identity;
using Shared.Logic.Validators;
using System.Text.Json;
using Shared.Models.Dtos.CommonNote;
using Shared.Data;
using Shared.Data.Converters;
using Microsoft.EntityFrameworkCore;

namespace Shared.Logic
{
    public static class LogicUtilities
    {
        //add logic layer specific utilities here...
        
        /// <summary>
        /// Generates a hashed password using ASP.NET Core Identity's PasswordHasher.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            var hasher = new PasswordHasher<object>();
            string passwordHash = hasher.HashPassword(user: null, password: password);
            return passwordHash;
        }

        /// <summary>
        /// Applies Active/IncludeInactive filtering.
        /// If IncludeInactive is false, only Active records are returned.
        /// </summary>
        public static IQueryable<TEntity> ApplyIncludeInactiveFilter<TEntity, TFilter>(
            this IQueryable<TEntity> query,
            TFilter filter)
            where TEntity : AuditableEntity
            where TFilter : BaseLogicGet
        {
            if (filter is null || filter.IncludeInactive)
            {
                return query;
            }

            return query.Where(x => x.Active == true);
        }

        /// <summary>
        /// Applies ReadOnly/IncludeReadOnly filtering.
        /// If IncludeReadOnly is false, only non-read-only records are returned.
        /// </summary>
        public static IQueryable<TEntity> ApplyIncludeReadOnlyFilter<TEntity, TFilter>(
            this IQueryable<TEntity> query,
            TFilter filter)
            where TEntity : AuditableEntity
            where TFilter : BaseLogicGet
        {
            if (filter is null || filter.IncludeReadOnly)
            {
                return query;
            }

            return query.Where(x => x.ReadOnly == false);
        }

        /// <summary>
        /// Applies filters based on auditable properties to the query. (IE: CreatedBy, CreatedOnDate, UpdatedBy, UpdatedOnDate)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ApplyAuditableFilters<TEntity, TFilter>(this IQueryable<TEntity> query, TFilter filter)
            where TEntity : AuditableEntity
            where TFilter : IAuditableFilter
        {
            if (filter is null) return query;

            if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }
                
            if (filter.CreatedOnDate.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedOn) == filter.CreatedOnDate);
            }

            if (!string.IsNullOrWhiteSpace(filter.UpdatedBy))
            {
                query = query.Where(x => x.UpdatedBy == filter.UpdatedBy);
            }
                
            if (filter.UpdatedOnDate.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime((DateTime)x.UpdatedOn) == filter.UpdatedOnDate);
            }

            return query;
        }

        public static Dictionary<string, List<string>> AddRecordNotFoundErrorToErrorValidationResult(Dictionary<string, List<string>> errors, string keyName, string idFieldName)
        {
            errors.Add(keyName, new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage(idFieldName) });
            return errors;
        }

        /// <summary>
        /// Parses a JSON string column into its actual structure so it nests correctly instead of being double-serialized as an escaped string.
        /// </summary>
        public static object? ParseJsonOrNull(string? json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<object>(json);
        }

        // public static class CommonNoteUtilities
        // {
        //     public static async Task<List<CommonNoteDto>> GetAllCommonNotesByReferenceAsync(string referenceType, int referenceId, ICommonNoteDbContext dbContext)
        //     {
        //         var commonNotes = await dbContext.CommonNotes
        //             .Where(n => n.ReferenceType == referenceType && n.ReferenceId == referenceId)
        //             .ToListAsync();

        //         return commonNotes.Select(n => n.ToDto()).ToList();
        //     }
            
        //     public static async Task InsertUpdateCommonNotes(List<InsertUpdateCommonNoteRequest>? commonNotes, 
        //                                                      string referenceType, 
        //                                                      int referenceId,
        //                                                      string currentUser, 
        //                                                      ICommonNoteDbContext dbContext
        //                                                     )
        //     {
        //         if (commonNotes != null)
        //         {
        //             var commonNotesEntities = new List<CommonNote>();
        //             var existingNotes = await dbContext.CommonNotes
        //                 .Where(n => n.ReferenceType == referenceType && n.ReferenceId == referenceId)
        //                 .ToDictionaryAsync(n => n.CommonNoteId);

        //             foreach (var commonNote in commonNotes)
        //             {
        //                 if (commonNote.CommonNoteId == null)
        //                 {
        //                     commonNotesEntities.Add(commonNote.ToEntityOnInsert(referenceType, referenceId, currentUser));
        //                 }
        //                 else
        //                 {
        //                     if (existingNotes.TryGetValue(commonNote.CommonNoteId.Value, out var existing))
        //                     {
        //                         existing.NoteType = commonNote.NoteType;
        //                         existing.Subject = commonNote.Subject;
        //                         existing.Text = commonNote.Text;
        //                         existing.CurrentUser = currentUser;
        //                     }
        //                 }
        //             }
                    
        //             await dbContext.CommonNotes.AddRangeAsync(commonNotesEntities);
        //         }
        //     }

        //     public static void DeleteAllCommonNotes(string referenceType, 
        //                                             int referenceId,
        //                                             string currentUser, 
        //                                             ICommonNoteDbContext dbContext
        //                                             )
        //     {
        //         dbContext.CommonNotes.RemoveRange(dbContext.CommonNotes.Where(note => note.ReferenceType == referenceType && note.ReferenceId == referenceId));

        //         //Todo: Delete log for common note?
        //     }
        // }
    }
}
