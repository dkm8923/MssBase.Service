using Shared.Models.Contracts;

namespace Shared.Data.Models;

public abstract class AuditableEntity: IActivatable, IReadOnly, ICreateable, IUpdateable
{
    public bool Active { get; set; } = true;
    public bool ReadOnly { get; set; } = false;
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}