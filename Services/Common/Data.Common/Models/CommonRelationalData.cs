using Shared.Data.Models;

namespace Data.Common.Models;

public partial class CommonRelationalData : AuditableEntity
{
    public int CommonRelationalDataId { get; set; }

    public string ReferenceType { get; set; }
    public string Description { get; set; }

    public string Json { get; set; }
}
