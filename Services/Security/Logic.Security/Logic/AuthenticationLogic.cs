using System;
using Contract.Security.Authentication;
using Dto.Security.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using Logic.Security.Validators.Authentication;
using Shared.Models;
using FluentValidation.Results;
using Shared.Logic.Validators;
using Contract.Security.Application;

namespace Logic.Security.Logic;

public class AuthenticationLogic : IAuthenticationLogic
{
    private readonly IOptionsMonitor<JwtAuthenticationConfig> _jwtConfigMonitor;
    

    private IValidator<AuthenticationRequest> _authenticationRequestValidator;

    public AuthenticationLogic(IOptionsMonitor<JwtAuthenticationConfig> jwtConfigMonitor, IValidator<AuthenticationRequest> authenticationRequestValidator)
    {
        _jwtConfigMonitor = jwtConfigMonitor;
        _authenticationRequestValidator = authenticationRequestValidator;
    }

    public async Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic)
    {
        var errorValidationResult = await _validateAuthenticationRequest(req, applicationLogic);
        if (errorValidationResult.Errors.Count > 0)
        {
            return errorValidationResult;
        }

        var userFilterRes = await applicationUserLogic.Filter(new FilterApplicationUserLogicRequest { Email = req.EmailAddress, ApplicationId = req.ApplicationId });

        var userNotFound = userFilterRes.Errors.Count() > 0 || userFilterRes.Response is null || !userFilterRes.Response.Any();
        //verify password - this is where you would implement your password hashing and verification logic
        var invalidPass = !userNotFound && userFilterRes.Response.First().Password != req.Password;
        
        if (userNotFound || invalidPass)
        {
            // errorValidationResult.Errors.Add("Authentication", new List<string> { "Invalid email address or password." });
            // return errorValidationResult;
            return new ErrorValidationResult<AuthenticationResponse>();
        }

        //auth successful, generate JWT token and return
        return new ErrorValidationResult<AuthenticationResponse> { Response = new AuthenticationResponse { Token = _generateJwtToken() } };
    }

    private async Task<ErrorValidationResult<AuthenticationResponse>> _validateAuthenticationRequest(AuthenticationRequest req,  IApplicationLogic applicationLogic)
    {
        ValidationResult result = await _authenticationRequestValidator.ValidateAsync(req);
        var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<AuthenticationResponse>(result);

        if (errorValidationResult.Errors.Count == 0)
        {
            // Validate Application exists
            var applicationIdCheck = await applicationLogic.GetById(req.ApplicationId, new BaseLogicGet { CurrentUser = req.EmailAddress});
                
            if (applicationIdCheck.Errors.Count > 0 || applicationIdCheck.Response == null)
            {
                errorValidationResult.Errors.Add("ApplicationId", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") });
            }
        }

        return errorValidationResult;
    }

    private string _generateJwtToken()
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

        return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    }
}
