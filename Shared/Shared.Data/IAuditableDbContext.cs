using Microsoft.EntityFrameworkCore;
using Shared.Data.Models;

namespace Shared.Data;

/// <summary>
/// Requires a DbContext to expose an AuditLog table, standardizing audit logging across services.
/// </summary>
public interface IAuditableDbContext
{
    DbSet<AuditLog> AuditLogs { get; set; }
}
