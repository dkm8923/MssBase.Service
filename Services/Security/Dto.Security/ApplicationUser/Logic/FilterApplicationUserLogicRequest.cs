using Shared.Models;

namespace Dto.Security.ApplicationUser.Logic
{
    public record FilterApplicationUserLogicRequest : BaseLogicGet
    {
        public string? CreatedBy { get; set; }

        public DateOnly? CreatedOnDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateOnly? UpdatedOnDate { get; set; }

        public List<int>? ApplicationUserIds { get; set; }

        public bool? Active { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? ApplicationId { get; set; }
    }
}
