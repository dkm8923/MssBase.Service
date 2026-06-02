using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.Authentication;
using Shared.Models;

namespace Contract.Security.Authentication;

public interface IAuthenticationLogic
{
    public Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic);
    public Task<ErrorValidationResult<AuthenticationResponse>> RefreshToken(RefreshTokenRequest req, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic);
    public Task<ErrorValidationResult> RevokeToken(RevokeTokenRequest req);
    public Task<ErrorValidationResult<NotificationMessageResponse>> ForgotPassword(ForgotPasswordRequest req, IApplicationUserLogic applicationUserLogic);
}
