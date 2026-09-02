using Shared.Models.Contracts;

namespace Dto.Security.User
{
    public record ChangePasswordRequest : ICurrentUser
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}