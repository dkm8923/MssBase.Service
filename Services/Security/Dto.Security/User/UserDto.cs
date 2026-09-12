using System.Text.Json.Serialization;
using Dto.Security.ApplicationUser;
using Shared.Models;
using Shared.Models.Contracts;
using Shared.Models.Dtos;

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
        public string? MaritalStatus { get; set; }
        public string? Religion { get; set; }
        public string? Gender { get; set; }
        public List<string>? SpokenLanguages { get; set; }
        public List<TypedValueDto>? PhoneNumbers { get; set; }
        public List<SocialMediaProfileDto>? SocialMediaProfiles { get; set; }
        public List<TypedValueDto>? AlternateEmails { get; set; }
        public List<CommonNoteDto>? CommonNotes { get; set; }
        
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
