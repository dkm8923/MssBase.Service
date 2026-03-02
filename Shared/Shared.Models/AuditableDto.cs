using Shared.Models.Contracts;

namespace Shared.Models;

public abstract record AuditableDto: ICreateable, IUpdateable
{
    public bool Active { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}
