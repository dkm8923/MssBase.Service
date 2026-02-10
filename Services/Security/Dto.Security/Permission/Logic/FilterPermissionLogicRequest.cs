using Shared.Models;

namespace Dto.Security.Permission.Logic
{
    public record FilterPermissionLogicRequest : BaseLogicGet
    {
        public string? CreatedBy { get; set; }

        public DateOnly? CreatedOnDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateOnly? UpdatedOnDate { get; set; }

        public List<int>? PermissionIds { get; set; }

        public bool? Active { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? ApplicationId { get; set; }
    }
}
