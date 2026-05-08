using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dto.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IOptionsMonitor<JwtAuthenticationConfig> _jwtConfigMonitor;

        public AuthenticationController(IOptionsMonitor<JwtAuthenticationConfig> jwtConfigMonitor)
        {
            _jwtConfigMonitor = jwtConfigMonitor;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticationRequest req)
        {
            if (req is null)
            {
                return BadRequest("Invalid client request");
            }
            if (req.Username == "johndoe" && req.Password == "def@123")
            {
                var jwtConfig = _jwtConfigMonitor.CurrentValue;

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: jwtConfig.ValidIssuer,
                    audience: jwtConfig.ValidAudience,
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(jwtConfig.TokenExpiryInMinutes),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new AuthenticatedResponse { Token = tokenString });
            }
            return Unauthorized();
        }
    }
}
