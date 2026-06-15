using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Common.CommonRelationalData.Logic
{
    public record FilterCommonRelationalDataLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<string>? ReferenceTypes { get; set; }
    }
}