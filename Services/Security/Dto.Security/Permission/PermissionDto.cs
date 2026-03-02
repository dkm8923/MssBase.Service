using Shared.Models;

namespace Dto.Security.Permission
{
    public record PermissionDto : AuditableDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ApplicationId { get; set; }
    }
}
