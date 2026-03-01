using Shared.Data.Models;

namespace Data.Security.Models;

public partial class ApplicationUserPermission : AuditableEntity
{
    public int ApplicationUserPermissionId { get; set; }

    public int ApplicationId { get; set; }

    public int ApplicationUserId { get; set; }

    public int PermissionId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}
