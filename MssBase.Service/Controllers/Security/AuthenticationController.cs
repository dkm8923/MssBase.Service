using Dto.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Contract.Security.Authentication;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using MssBase.Service.Controllers.Shared;

namespace MssBase.Service.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Authentication")]
    [AutoValidationAttribute]
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest req)
        {
            var result = await _authenticationService.Authenticate(req);
            
            if (result.Errors.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            var result = await _authenticationService.RefreshToken(req);
            
            if (result.Errors.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest req)
        {
            var result = await _authenticationService.RevokeToken(req);
            
            if (result.Errors.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
