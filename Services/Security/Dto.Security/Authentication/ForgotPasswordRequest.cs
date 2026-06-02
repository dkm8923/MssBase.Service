using Shared.Models.Contracts;

namespace Dto.Security.Authentication;

public record ForgotPasswordRequest : ICurrentUser
{
    public string Email { get; set; }
    public string CurrentUser { get; set; }
}