using System.Text.Json.Serialization;
using Dto.Security.ApplicationUser;
using Shared.Models;
using Shared.Models.Contracts;

namespace Dto.Security.User
{
    public record UserDto : AuditableDto, IPerson
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreferredName { get; set; }
        public string? Suffix { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? TimeZone { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }
        public bool PasswordResetRequired { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastPasswordChangeDateTime { get; set; }
        public DateTime? LastLockoutDateTime { get; set; }
        public short? FailedPasswordAttemptCount { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserDto> ApplicationUsers { get; set; } = null!;
    }
}
