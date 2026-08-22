using Shared.Models.Contracts;

namespace Shared.Models.Dtos;

public record AuditLogDto: ICreateable
{
    public int AuditLogId { get; set; }
    public string LogType { get; set; }
    public string ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public dynamic ChangeLogJson { get; set; }
    public dynamic RecordStateBeforeChangeJson { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
}
