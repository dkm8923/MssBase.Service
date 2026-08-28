using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser
{
    public record ChangePasswordRequest : ICurrentUser
    {
        public int ApplicationUserId { get; set; }
        public string NewPassword { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}