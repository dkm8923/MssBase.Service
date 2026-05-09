namespace Dto.Security.Authentication;

public record AuthenticationRequest
{
    public int ApplicationId { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
}
