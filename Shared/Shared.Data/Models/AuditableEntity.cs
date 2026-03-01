namespace Shared.Data.Models;

public abstract class AuditableEntity
{
    public bool Active { get; set; } = true;
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}