using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUserPermission.Logic
{
    public record FilterApplicationUserPermissionLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? ApplicationUserPermissionIds { get; set; }
        public int? ApplicationId { get; set; }
        public int? ApplicationUserId { get; set; }
        public int? PermissionId { get; set; }
    }
}
