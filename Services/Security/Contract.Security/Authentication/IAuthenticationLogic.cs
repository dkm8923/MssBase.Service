using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.Authentication;
using Shared.Models;

namespace Contract.Security.Authentication;

public interface IAuthenticationLogic
{
    Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic);
}
