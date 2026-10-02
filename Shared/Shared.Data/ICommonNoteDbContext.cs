using Microsoft.EntityFrameworkCore;
using Shared.Data.Models;

namespace Shared.Data;

/// <summary>
/// Requires a DbContext to expose a CommonNote table, standardizing common note management across services.
/// </summary>
public interface ICommonNoteDbContext
{
    DbSet<CommonNote> CommonNotes { get; set; }
}
