namespace Dto.Security.Authentication;

public record AuthenticationRequest
{
    public int ApplicationId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
