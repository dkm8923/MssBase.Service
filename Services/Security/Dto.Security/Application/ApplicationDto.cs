using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.Permission;
using Dto.Security.Role;
using Dto.Security.RolePermission;
using Shared.Models;

namespace Dto.Security.Application
{
    public record ApplicationDto : AuditableDto
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IEnumerable<ApplicationUserDto> ApplicationUsers { get; set; }
        public IEnumerable<PermissionDto> Permissions { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
        public IEnumerable<RolePermissionDto> RolePermissions { get; set; }
        public IEnumerable<ApplicationUserPermissionDto> ApplicationUserPermissions { get; set; }
    }
}
