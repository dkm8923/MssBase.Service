using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser.Logic
{
    public record FilterApplicationUserLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? ApplicationUserIds { get; set; }
        public int? UserId { get; set; }
        public int? ApplicationId { get; set; }
    }
}
