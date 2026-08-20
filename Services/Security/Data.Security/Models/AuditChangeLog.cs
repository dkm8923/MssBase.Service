using Shared.Models.Contracts;

namespace Data.Security.Models;

public partial class AuditChangeLog : ICreateable
{
    public int AuditChangeLogId { get; set; }
    public string ChangeType { get; set; }
    public string ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public string Json { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
}
