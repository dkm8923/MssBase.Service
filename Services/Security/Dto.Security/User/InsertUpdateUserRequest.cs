using Shared.Models.Contracts;

namespace Dto.Security.User
{
    public record InsertUpdateUserRequest : ICurrentUser
    {
        public string Email { get; set; } = null!;
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreferredName { get; set; }
        public string? Suffix { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? TimeZone { get; set; }
        public bool Active { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}
