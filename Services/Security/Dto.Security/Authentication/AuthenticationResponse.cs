namespace Dto.Security.Authentication;

public record AuthenticationResponse
{
    public string? Token { get; set; }
}
