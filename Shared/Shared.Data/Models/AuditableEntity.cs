using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models.Contracts;

namespace Shared.Data.Models;

public abstract class AuditableEntity: IActivatable, IReadOnly, ICreateable, IUpdateable, ICurrentUser
{
    public bool Active { get; set; } = true;
    public bool ReadOnly { get; set; } = false;
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Transient, not persisted. Set by callers so AuditableEntitySaveChangesInterceptor can stamp CreatedBy/UpdatedBy on save.
    /// </summary>
    [NotMapped]
    public string CurrentUser { get; set; }
}