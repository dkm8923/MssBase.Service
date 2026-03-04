using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.Application.Logic
{
    public record FilterApplicationLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? ApplicationIds { get; set; }
        public string? Name { get; set; }
    }
}
