using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser
{
    public record InsertUpdateApplicationUserRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Password { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public DateTime? LastLockoutDate { get; set; }

        public short? FailedPasswordAttemptCount { get; set; }

        public int ApplicationId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
