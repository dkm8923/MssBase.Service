using Shared.Models.Contracts;

public record RevokeTokenRequest : ICurrentUser
{
    public string Email { get; set; }
    public string CurrentUser { get; set; }
}