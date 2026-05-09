using Dto.Security.Authentication;
using Shared.Models;

namespace Contract.Security.Authentication;

public interface IAuthenticationService
{
    Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req);
}
