using Shared.Models.Contracts;

namespace Dto.Security.RolePermission
{
    public record InsertUpdateRolePermissionRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public int ApplicationId { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
