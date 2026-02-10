using Shared.Models.Contracts;

namespace Dto.Security.UserPermission
{
    public record UserPermissionDto// : ICreateable, IUpdateable
    {
        public int UserPermissionId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public bool Active { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationUserId { get; set; }

        public int PermissionId { get; set; }
    }
}
