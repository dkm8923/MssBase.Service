using System.Text.Json.Serialization;
using Dto.Security.Permission;
using Shared.Models;

namespace Dto.Security.RolePermission
{
    public record RolePermissionDto : AuditableDto
    {
        public int RolePermissionId { get; set; }
        public int ApplicationId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionDto Permission { get; set; }
    }
}
