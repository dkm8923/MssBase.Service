using Shared.Models;

namespace Dto.Security.UserPermission.Logic
{
    public record FilterUserPermissionLogicRequest : BaseLogicGet
    {
        public string? CreatedBy { get; set; }

        public DateOnly? CreatedOnDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateOnly? UpdatedOnDate { get; set; }

        public List<int>? UserPermissionIds { get; set; }

        public bool? Active { get; set; }

        public int? ApplicationId { get; set; }

        public int? ApplicationUserId { get; set; }

        public int? PermissionId { get; set; }
    }
}
