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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }
        public bool PasswordResetRequired { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public short? FailedPasswordAttemptCount { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserDto> ApplicationUsers { get; set; } = null!;
    }
}
