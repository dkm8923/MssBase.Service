using Shared.Models;

namespace Dto.Security.ApplicationUserRole
{
    public record ApplicationUserRoleDto : AuditableDto
    {
        public int ApplicationUserRoleId { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationUserId { get; set; }
        public int RoleId { get; set; }
    }
}
