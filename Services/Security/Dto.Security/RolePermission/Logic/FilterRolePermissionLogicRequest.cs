using Shared.Models;

namespace Dto.Security.RolePermission.Logic
{
    public record FilterRolePermissionLogicRequest : BaseLogicGet
    {
        public string? CreatedBy { get; set; }

        public DateOnly? CreatedOnDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateOnly? UpdatedOnDate { get; set; }

        public List<int>? RolePermissionIds { get; set; }

        public bool? Active { get; set; }

        public int? ApplicationId { get; set; }

        public int? RoleId { get; set; }

        public int? PermissionId { get; set; }
    }
}
