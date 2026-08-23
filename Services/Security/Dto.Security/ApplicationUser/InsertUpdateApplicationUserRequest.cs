using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser
{
    public record InsertUpdateApplicationUserRequest : ICurrentUser
    {
        

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }

        public int ApplicationId { get; set; }

        public bool Active { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}
