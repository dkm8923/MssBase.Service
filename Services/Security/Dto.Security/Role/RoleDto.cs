using Shared.Models;
using Dto.Security.RolePermission;
using System.Text.Json.Serialization;

namespace Dto.Security.Role
{
    public record RoleDto : AuditableDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ApplicationId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<RolePermissionDto>? RolePermissions { get; set; }
    }
}
