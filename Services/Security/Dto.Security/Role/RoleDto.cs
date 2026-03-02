using Shared.Models;

namespace Dto.Security.Role
{
    public record RoleDto : AuditableDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ApplicationId { get; set; }
    }
}
