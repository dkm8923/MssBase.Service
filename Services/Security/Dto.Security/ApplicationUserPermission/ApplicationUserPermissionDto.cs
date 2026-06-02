using System.Text.Json.Serialization;
using Dto.Security.Permission;
using Shared.Models;

namespace Dto.Security.ApplicationUserPermission
{
    public record ApplicationUserPermissionDto : AuditableDto
    {
        public int ApplicationUserPermissionId { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationUserId { get; set; }
        public int PermissionId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionDto Permission { get; set; }
    }
}
