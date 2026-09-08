using Shared.Models.Contracts;
using Shared.Models.Dtos;

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
        public DateOnly? DateOfBirth { get; set; }
        public string? TimeZone { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Religion { get; set; }
        public string? Sexuality { get; set; }
        public string? Gender { get; set; }
        public List<string>? SpokenLanguages { get; set; }
        public List<TypedValueDto>? PhoneNumbers { get; set; }
        public List<SocialMediaProfileDto>? SocialMediaProfiles { get; set; }
        public List<TypedValueDto>? AlternateEmails { get; set; }
        public bool Active { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}
