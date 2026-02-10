using Shared.Models.Contracts;

namespace Dto.Security.RolePermission
{
    public record RolePermissionDto// : ICreateable, IUpdateable
    {
        public int RolePermissionId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public bool Active { get; set; }

        public int ApplicationId { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }
    }
}
