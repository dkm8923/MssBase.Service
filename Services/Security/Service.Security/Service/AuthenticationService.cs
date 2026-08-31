using Contract.Security.Authentication;
using Dto.Security.Authentication;
using Contract.Security.User;
using Contract.Security.Application;
using Shared.Models;

namespace Service.Security.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationLogic _authenticationLogic;
    private readonly IUserLogic _userLogic;
    private readonly IApplicationLogic _applicationLogic;

    public AuthenticationService(IAuthenticationLogic authenticationLogic, IUserLogic userLogic, IApplicationLogic applicationLogic)
    {
        _authenticationLogic = authenticationLogic;
        _userLogic = userLogic;
        _applicationLogic = applicationLogic;
    }

    public async Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req)
    {
        return await _authenticationLogic.Authenticate(req, _userLogic, _applicationLogic);
    }
    
    public async Task<ErrorValidationResult<AuthenticationResponse>> RefreshToken(RefreshTokenRequest req)
    {
        return await _authenticationLogic.RefreshToken(req, _userLogic, _applicationLogic);
    }

    public async Task<ErrorValidationResult> RevokeToken(RevokeTokenRequest req)
    {
        return await _authenticationLogic.RevokeToken(req);
    }

    public async Task<ErrorValidationResult<NotificationMessageResponse>> ForgotPassword(ForgotPasswordRequest req)
    {
        return await _authenticationLogic.ForgotPassword(req, _userLogic);
    }
}
