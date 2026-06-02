using Dto.Security.Authentication;
using Shared.Models;

namespace Contract.Security.Authentication;

public interface IAuthenticationService
{
    public Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req);
    public Task<ErrorValidationResult<AuthenticationResponse>> RefreshToken(RefreshTokenRequest req);
    public Task<ErrorValidationResult> RevokeToken(RevokeTokenRequest req);
    public Task<ErrorValidationResult<NotificationMessageResponse>> ForgotPassword(ForgotPasswordRequest req);
}
