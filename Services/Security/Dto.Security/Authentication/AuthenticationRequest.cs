namespace Dto.Security.Authentication;

public record AuthenticationRequest
{
    public string ApplicationName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
