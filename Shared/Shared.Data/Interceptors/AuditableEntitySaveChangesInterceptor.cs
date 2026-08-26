using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Data.Models;

namespace Shared.Data.Interceptors;

/// <summary>
/// Stamps CreatedOn/CreatedBy/UpdatedOn/UpdatedBy on <see cref="AuditableEntity"/> instances before they are saved,
/// removing the need for Converters to set these fields manually.
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetAuditFields(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAuditFields(DbContext context)
    {
        if (context == null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = utcNow;
                entry.Entity.CreatedBy = entry.Entity.CurrentUser;
                entry.Entity.UpdatedOn = utcNow;
                entry.Entity.UpdatedBy = entry.Entity.CurrentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedOn = utcNow;
                entry.Entity.UpdatedBy = entry.Entity.CurrentUser;
            }
        }
    }
}
