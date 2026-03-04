using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.RolePermission.Logic
{
    public record FilterRolePermissionLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? RolePermissionIds { get; set; }
        public int? ApplicationId { get; set; }
        public int? RoleId { get; set; }
        public int? PermissionId { get; set; }
    }
}
