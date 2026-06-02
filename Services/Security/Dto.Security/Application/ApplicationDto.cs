using System.Text.Json.Serialization;
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
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserDto> ApplicationUsers { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<PermissionDto> Permissions { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<RoleDto> Roles { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<RolePermissionDto> RolePermissions { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserPermissionDto> ApplicationUserPermissions { get; set; }
    }
}
