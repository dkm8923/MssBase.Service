using Shared.Models.Contracts;

namespace Shared.Data.Models;

public partial class AuditLog : ICreateable
{
    public int AuditLogId { get; set; }
    public string LogType { get; set; }
    public string ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public string ChangeLogJson { get; set; }
    public string RecordStateBeforeChangeJson { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
}
