using System.Text.Json.Serialization;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserRole;
using Shared.Models;

namespace Dto.Security.ApplicationUser
{
    public record ApplicationUserDto : AuditableDto
    {
        public int ApplicationUserId { get; set; }
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Password { get; set; }
        public bool PasswordResetRequired { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public short? FailedPasswordAttemptCount { get; set; }
        public int ApplicationId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserPermissionDto> ApplicationUserPermissions { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ApplicationUserRoleDto> ApplicationUserRoles { get; set; }
    }
}
