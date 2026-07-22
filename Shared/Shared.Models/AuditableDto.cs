using Shared.Models.Contracts;

namespace Shared.Models;

public abstract record AuditableDto: IActivatable, IReadOnly, ICreateable, IUpdateable
{
    public bool Active { get; set; }
    public bool ReadOnly { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}
