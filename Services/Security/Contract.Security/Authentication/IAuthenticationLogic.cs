using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.User;
using Dto.Security.Authentication;
using Shared.Models;

namespace Contract.Security.Authentication;

public interface IAuthenticationLogic
{
    public Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req, IUserLogic userLogic, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic);
    public Task<ErrorValidationResult<AuthenticationResponse>> RefreshToken(RefreshTokenRequest req, IUserLogic userLogic, IApplicationLogic applicationLogic);
    public Task<ErrorValidationResult> RevokeToken(RevokeTokenRequest req);
    public Task<ErrorValidationResult<NotificationMessageResponse>> ForgotPassword(ForgotPasswordRequest req, IUserLogic userLogic);
}
