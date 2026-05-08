namespace Dto.Security.Authentication;

public record AuthenticationRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
