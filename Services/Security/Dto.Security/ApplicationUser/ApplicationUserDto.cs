using System.Text.Json.Serialization;
using Dto.Security.Application;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserRole;
using Shared.Models;

namespace Dto.Security.ApplicationUser
{
    public record ApplicationUserDto : AuditableDto
    {
        public int ApplicationUserId { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserPermissionDto> ApplicationUserPermissions { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserRoleDto> ApplicationUserRoles { get; set; }
    }
}
