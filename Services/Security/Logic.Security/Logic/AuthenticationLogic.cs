using Contract.Security.Authentication;
using Dto.Security.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Contract.Security.ApplicationUser;
using FluentValidation;
using Shared.Models;
using FluentValidation.Results;
using Shared.Logic.Validators;
using Contract.Security.Application;
using Data.Security;
using Microsoft.EntityFrameworkCore;
using Contract.Security;
using Shared.Logic.Common;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser;
using System.Text.Json;
using Dto.Security.Application.Logic;
using Dto.Security.Application;

namespace Logic.Security.Logic;

public class AuthenticationLogic : IAuthenticationLogic
{
    private readonly IOptionsMonitor<JwtAuthenticationConfig> _jwtConfigMonitor;
    private readonly IOptionsMonitor<AuthenticationSettingsConfig> _authenticationSettingsConfigMonitor;
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;

    private IValidator<AuthenticationRequest> _authenticationRequestValidator;

    private int _maxFailedPasswordAttemptCount => _authenticationSettingsConfigMonitor.CurrentValue.MaxFailedPasswordAttemptCount > 0 ? _authenticationSettingsConfigMonitor.CurrentValue.MaxFailedPasswordAttemptCount : 5;
    private int _lockoutDurationInMinutes => _authenticationSettingsConfigMonitor.CurrentValue.LockoutDurationInMinutes > 0 ? _authenticationSettingsConfigMonitor.CurrentValue.LockoutDurationInMinutes : 60;
    private int _passwordExpiryInDays => _authenticationSettingsConfigMonitor.CurrentValue.PasswordExpiryInDays > 0 ? _authenticationSettingsConfigMonitor.CurrentValue.PasswordExpiryInDays : 90;

    public AuthenticationLogic(
                    IOptionsMonitor<JwtAuthenticationConfig> jwtConfigMonitor, 
                    IOptionsMonitor<AuthenticationSettingsConfig> authenticationSettingsConfigMonitor, 
                    ISecurityConnectionStrings connectionStrings, 
                    IValidator<AuthenticationRequest> authenticationRequestValidator
    )
    {
        _jwtConfigMonitor = jwtConfigMonitor;
        _authenticationSettingsConfigMonitor = authenticationSettingsConfigMonitor;
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
        _authenticationRequestValidator = authenticationRequestValidator;
    }

    public async Task<ErrorValidationResult<AuthenticationResponse>> Authenticate(AuthenticationRequest req, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic)
    {
        var applicationRes = await _retrieveApplicationInfoForAuthentication(req, applicationLogic);

        var errorValidationResult = await _validateAuthenticationRequest(req, applicationRes);
        if (errorValidationResult.Errors.Count > 0)
        {
            return errorValidationResult;
        }

        var userInfoRes = await _retrieveRequiredUserInfoForAuthentication(req.Email, applicationRes.ApplicationId);

        if (userInfoRes is null)
        {
            //user not found with that email address / application id combo
            return _createInvalidCredentialsError();
        }

        //check if user is currently locked out due to too many failed password attempts. If so, return lockout message instead of invalid credentials message
        if (userInfoRes.LastLockoutDate.HasValue && userInfoRes.LastLockoutDate.Value.AddMinutes(_lockoutDurationInMinutes) > CommonUtilities.GetDateTimeUtcNow())
        {
            //user is currently locked out
            return _createAccountLockedError();
        }
        
        var isValidPassword = SecurityLogicUtilities.VerifyPasswordMatchesHash(userInfoRes.PasswordHash, req.Password);

        if (!isValidPassword)
        {
            var failedPasswordAttemptCount = await _updateFailedPasswordAttemptLogic(userInfoRes.ApplicationUserId);

            if (failedPasswordAttemptCount >= _maxFailedPasswordAttemptCount)
            {
                //user is locked out for configuration defined duration, return lockout message instead of invalid credentials message
                return _createAccountLockedError();
            }

            return _createInvalidCredentialsError();
        }

        var pswdChangeRequired = userInfoRes.PasswordResetRequired == true || userInfoRes.LastPasswordChangeDate.Value.AddDays(_passwordExpiryInDays) < CommonUtilities.GetDateTimeUtcNow();
        
        if (pswdChangeRequired)
        {
            //password change is required, return password change required error
            return _createPasswordChangeRequiredError();
        }

        //successful auth occurred, update user accordingly 
        await _updateApplicationUserOnSuccessfulLogin(userInfoRes.ApplicationUserId);

        //generate JWT token and return
        var applicationUserWithRelatedData = await applicationUserLogic.GetById(userInfoRes.ApplicationUserId, new BaseLogicGet { CurrentUser = userInfoRes.Email, IncludeRelated = true });
        
        var authCredentials = _extractAuthorizationCredentialsFromApplicationUserResponse(applicationUserWithRelatedData.Response, applicationRes);

        return new ErrorValidationResult<AuthenticationResponse> { Response = new AuthenticationResponse { Token = _generateJwtToken(authCredentials) } };
    }

    // public async Task<ErrorValidationResult<NotificationMessageResponse>> ForgotUserName(string emailAddress, IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic, CancellationToken cancellationToken = default)
    // {
    //     var userRes = await applicationUserLogic.Filter(new FilterApplicationUserLogicRequest { Email = emailAddress, CurrentUser = emailAddress  }, cancellationToken);

    //     if (userRes.Errors.Count > 0 || userRes.Response is null || userRes.Response.Count() == 0)
    //     {
    //         //to prevent user enumeration attacks, return success message even if email address does not exist in the system
    //         return new ErrorValidationResult<NotificationMessageResponse> { Response = new NotificationMessageResponse { Message = "If an account with that email address exists, a notification email has been sent with the username." } };
    //     }
    // }

    // public record NotificationMessageResponse
    // {
    //     public string Message { get; set; }
    // }

    #region private

    /// <summary>
    /// Creates an error result indicating that the provided email address or password is invalid. The error is associated with a general "Authentication" key to avoid revealing whether the email address or the password was incorrect, which is a security best practice to prevent user enumeration attacks.
    /// </summary>
    /// <returns></returns>
    private ErrorValidationResult<AuthenticationResponse> _createInvalidCredentialsError()
    {
        return new ErrorValidationResult<AuthenticationResponse> { Errors = new Dictionary<string, List<string>> { { "Authentication", new List<string> { "Invalid email address or password!" } } } };
    }

    /// <summary>
    /// Creates an error result indicating that the account is locked due to too many failed login attempts. The error message includes the duration of the lockout period based on the configured lockout duration. This method is used to provide a clear error message to the user when their account is locked, and to inform them of how long they need to wait before they can attempt to log in again.
    /// </summary>
    /// <returns></returns>
    private ErrorValidationResult<AuthenticationResponse> _createAccountLockedError()
    {
        return new ErrorValidationResult<AuthenticationResponse> { Errors = new Dictionary<string, List<string>> { { "Authentication", new List<string> { $"Account is locked due to too many failed login attempts. Please try again after {_lockoutDurationInMinutes} minutes!" } } } };
    }

    /// <summary>
    /// Creates an error result indicating that the provided credentials are invalid because a password change is required. The error message informs the user that they need to update their password. This method is used to provide a clear error message to the user when their password has expired and needs to be changed, while still using a general "Authentication" key to avoid revealing that the email address was valid.
    /// </summary>
    /// <returns></returns>
    private ErrorValidationResult<AuthenticationResponse> _createPasswordChangeRequiredError()
    {
        return new ErrorValidationResult<AuthenticationResponse> { Errors = new Dictionary<string, List<string>> { { "Authentication", new List<string> { "Password change is required. Please update your password!" } } } };
    }

    /// <summary>
    /// Retrieves the application information for the authentication request based on the provided application name. This is used to validate that the application exists and to retrieve the application id needed for subsequent queries. If the application does not exist or there is an error during retrieval, this method returns null. This method is used as part of the authentication process to ensure that the authentication request is associated with a valid application in the system.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="applicationLogic"></param>
    /// <returns></returns>
    private async Task<ApplicationDto> _retrieveApplicationInfoForAuthentication(AuthenticationRequest req, IApplicationLogic applicationLogic)
    {
        var applicationRes = await applicationLogic.Filter(new FilterApplicationLogicRequest { Name = req.ApplicationName, CurrentUser = req.Email });

        if (applicationRes.Errors.Count > 0 || applicationRes.Response == null || applicationRes.Response.Count() == 0)
        {
            return null;
        }

        return applicationRes.Response.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the required user info for authentication (application user id, email address, password hash). This is done in a single query for efficiency and to avoid loading unnecessary data. The password hash is needed to verify the provided password.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private async Task<RequiredUserInfoForAuthenticationResponse> _retrieveRequiredUserInfoForAuthentication(string email, int applicationId)
    {
        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            var query = dbContext.ApplicationUsers.AsQueryable().AsNoTracking();
            var user = await query.Where(x => x.ApplicationId == applicationId && x.Email == email && x.Active)
                                  .Select(au => new { 
                                    au.ApplicationUserId, 
                                    au.Email, 
                                    au.Password,
                                    au.PasswordResetRequired, 
                                    au.LastLockoutDate, 
                                    au.LastPasswordChangeDate
                                  })
                                  .FirstOrDefaultAsync();

             if (user == null)
             {
                 return null;
             }

             return new RequiredUserInfoForAuthenticationResponse { 
                ApplicationUserId = user.ApplicationUserId, 
                Email = user.Email, 
                PasswordHash = user.Password, 
                PasswordResetRequired = user.PasswordResetRequired, 
                LastLockoutDate = user.LastLockoutDate, 
                LastPasswordChangeDate = user.LastPasswordChangeDate
            };
        }
    }

    /// <summary>
    /// Validates the authentication request using FluentValidation, and also checks if the provided application id exists. Returns an ErrorValidationResult containing any validation errors. If there are no validation errors, the Errors dictionary will be empty. This method is used to ensure that the authentication request is valid before attempting to authenticate the user.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="applicationRes"></param>
    /// <returns></returns>
    private async Task<ErrorValidationResult<AuthenticationResponse>> _validateAuthenticationRequest(AuthenticationRequest req,  ApplicationDto applicationRes)
    {
        ValidationResult result = await _authenticationRequestValidator.ValidateAsync(req);
        var errorValidationResult = ValidatorUtilities.CreateDefaultValidationResponse<AuthenticationResponse>(result);

        if (errorValidationResult.Errors.Count == 0)
        {
            // Validate Application exists
            if (applicationRes == null)
            {
                errorValidationResult.Errors.Add("ApplicationName", new List<string> { ValidatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationName") });
            }
        }

        return errorValidationResult;
    }

    /// <summary>
    /// Increments the failed password attempt count for the application user. If the failed password attempt count exceeds the configured maximum, also sets the last lockout date to the current date and time to indicate that the user is locked out. Returns the updated failed password attempt count after incrementing. This method is used to track failed login attempts and lock out users who exceed the maximum allowed attempts to help prevent brute force attacks.
    /// </summary>
    /// <param name="applicationUserId"></param>
    /// <returns></returns>
    private async Task<short> _updateFailedPasswordAttemptLogic(int applicationUserId)
    {
        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);

            if (entity != null)
            {
                entity.FailedPasswordAttemptCount = (short)(entity.FailedPasswordAttemptCount + 1);

                if (entity.FailedPasswordAttemptCount >= _maxFailedPasswordAttemptCount)
                {
                    entity.LastLockoutDate = CommonUtilities.GetDateTimeUtcNow();
                }

                await dbContext.SaveChangesAsync();
                return (short)entity.FailedPasswordAttemptCount;
            }
        }

        return 0;
    }

    /// <summary>
    /// Updates the application user's last login date to the current date and time, and resets the failed password attempt count to 0 on successful login.
    /// </summary>
    /// <param name="applicationUserId"></param>
    /// <returns></returns>
    private async Task _updateApplicationUserOnSuccessfulLogin(int applicationUserId)
    {
        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == applicationUserId);

            if (entity != null)
            {
                entity.LastLoginDate = CommonUtilities.GetDateTimeUtcNow();
                entity.FailedPasswordAttemptCount = 0; //reset failed password attempt count on successful login

                await dbContext.SaveChangesAsync();
            }
        }
    }
    
    private AuthorizationCredentialsResponse _extractAuthorizationCredentialsFromApplicationUserResponse(ApplicationUserDto applicationUser, ApplicationDto applicationRes)
    {
        var permissions = new List<string>();
        var roles = new List<string>();
        
        if (applicationUser.ApplicationUserPermissions != null)
        {
            foreach (var aup in applicationUser.ApplicationUserPermissions)
            {
                permissions.Add(aup.Permission.Name);
            }
        }
        
        if (applicationUser.ApplicationUserRoles != null)
        {
            foreach (var aur in applicationUser.ApplicationUserRoles)
            {
                roles.Add(aur.Role.Name);

                if (aur.Role.RolePermissions != null)
                {
                    foreach (var p in aur.Role.RolePermissions)
                    {
                        permissions.Add(p.Permission.Name);
                    }
                }
            }
        }
        
        permissions = permissions.Distinct().ToList();
        roles = roles.Distinct().ToList();

        return new AuthorizationCredentialsResponse
        {
            ApplicationName = applicationRes.Name,
            Email = applicationUser.Email,
            Permissions = permissions,
            Roles = roles
        };
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the authenticated user. The token includes claims, issuer, audience, and expiration information, and is signed using the configured signing key.
    /// </summary>
    /// <returns>A JWT as a string.</returns>
    private string _generateJwtToken(AuthorizationCredentialsResponse authCredentials)
    {
        var jwtConfig = _jwtConfigMonitor.CurrentValue;

        var applications = new List<string> { authCredentials.ApplicationName };

        var claims = new List<Claim>
        {
            // Keep them as separate arrays in the token payload
            new("user", JsonSerializer.Serialize(new { email = authCredentials.Email }), JsonClaimValueTypes.Json),
            new("applications", JsonSerializer.Serialize(applications ?? new List<string>()), JsonClaimValueTypes.JsonArray),
            new("roles", JsonSerializer.Serialize(authCredentials.Roles ?? new List<string>()), JsonClaimValueTypes.JsonArray),
            new("permissions", JsonSerializer.Serialize(authCredentials.Permissions ?? new List<string>()), JsonClaimValueTypes.JsonArray)
        };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: jwtConfig.ValidIssuer,
            audience: jwtConfig.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtConfig.TokenExpiryInMinutes),
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    }

    private record AuthorizationCredentialsResponse
    {
        public string ApplicationName { get; set; }
        public string Email { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }
    }

    private record RequiredUserInfoForAuthenticationResponse
    {
        public int ApplicationUserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public bool PasswordResetRequired { get; set; }
    }

    #endregion

    
}
