using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.Role.Logic
{
    public record FilterRoleLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? RoleIds { get; set; }
        public string? Name { get; set; }
        public int? ApplicationId { get; set; }
    }
}
