using Shared.Data.Models;

namespace Data.Security.Models;

public partial class RolePermission : AuditableEntity
{
    public int RolePermissionId { get; set; }

    public int ApplicationId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
