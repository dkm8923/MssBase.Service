using Shared.Data.Models;
using Shared.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Shared.Data.Converters;

public static class AuditLogConverters
{
    public static AuditLogDto ToDto(this AuditLog source)
    {
        if (source == null)
        {
            return null;
        }

        var target = new AuditLogDto
        {
            AuditLogId = source.AuditLogId,
            LogType = source.LogType,
            ReferenceType = source.ReferenceType,
            ReferenceId = source.ReferenceId,
            Json = source.Json,
            CreatedOn = source.CreatedOn,
            CreatedBy = source.CreatedBy
        };

        return target;
    }

    public static async Task<List<AuditLogDto>> ToDtos(this IQueryable<AuditLog> source, CancellationToken cancellationToken = default)
    {
        if (source == null)
        {
            return null;
        }

        var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

        return target;
    }
}