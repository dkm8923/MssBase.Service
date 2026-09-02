using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.User.Logic
{
    public record FilterUserLogicRequest : BaseLogicGet, IAuditableFilter
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<int>? UserIds { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreferredName { get; set; }
        public string? Suffix { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? TimeZone { get; set; }
        public int? ApplicationId { get; set; }
    }
}
