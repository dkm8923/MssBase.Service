namespace Dto.Security.Authentication;

public class JwtAuthenticationConfig
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string IssuerSigningKey { get; set; }
    public int TokenExpiryInMinutes { get; set; }
}
