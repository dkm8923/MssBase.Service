using Shared.Models.Contracts;

namespace Dto.Security.Authentication;

public record RevokeTokenRequest : ICurrentUser
{
    public string Email { get; set; }
    public string CurrentUser { get; set; }
}