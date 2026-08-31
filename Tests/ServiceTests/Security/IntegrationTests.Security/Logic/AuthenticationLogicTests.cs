using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Dto.Security.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Dto.Security.ApplicationUser;
using Dto.Security.User;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class AuthenticationLogicTests : SecurityTestBase
    {
        private int _maxFailedPasswordAttemptCount => _authenticationSettingsConfigMonitor.CurrentValue.MaxFailedPasswordAttemptCount;
        private int _passwordExpiryInDays => _authenticationSettingsConfigMonitor.CurrentValue.PasswordExpiryInDays;
        private int _refreshTokenExpiryInDays => _jwtAuthenticationConfigMonitor.CurrentValue.RefreshTokenExpiryInDays;

        #region Authenticate

        [Fact]
        public async Task Authenticate_Should_Authenticate_User()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);
            var newPassword = TestConstants.DefaultTestUserPassword;
            
            //change password after initial user creation
            await _userLogic.ChangePassword(new ChangePasswordRequest {
                UserId = testUser.Response.UserId,
                NewPassword = newPassword,
                CurrentUser = TestConstants.CurrentUser
            });

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Response.Email, newPassword);

            var testUserAfterSuccessfulAuthentication = await _userLogic.GetById(testUser.Response.UserId, new BaseLogicGet());

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Token.Should().NotBeNullOrEmpty();
            testUserAfterSuccessfulAuthentication.Response.FailedPasswordAttemptCount.Should().Be(0);
            testUserAfterSuccessfulAuthentication.Response.LastPasswordChangeDate.Should().NotBeNull();
            testUserAfterSuccessfulAuthentication.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            testUserAfterSuccessfulAuthentication.Response.LastLoginDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            //TODO: Decrypt token and verify claims once we have a way to do that in our tests
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_Email()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);
            
            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, "InvalidEmail@example.com", testUser.Response.Password);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidCredentialsErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_Password()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);
            
            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Response.Email, "InvalidPassword");

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidCredentialsErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_ApplicationId()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);
            
            // Act
            var result = await _authenticate("InvalidApplicationName", testUser.Response.Email, testUser.Response.Password);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidApplicationIdFieldErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Password_Reset_Required()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedPasswordChangeRequiredErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Response.Email, testUser.Response.Password);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _authenticate(null, null, null);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Field_Max_Length_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _authenticate(LogicTestUtilities.GenerateRandomString(65), LogicTestUtilities.GenerateRandomString(120) + "@test.com", LogicTestUtilities.GenerateRandomString(65));

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Account_Locked()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedAccountLockedErrors();

            //attempt to authenticate with invalid password until account is locked
            for (int i = 0; i < _maxFailedPasswordAttemptCount; i++)
            {                
                await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, "InvalidPassword");
            }

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);
            
            var testUserAfterFailedAuthenticationAttempt = await _userLogic.GetById(testUser.UserId, new BaseLogicGet());

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);

            testUserAfterFailedAuthenticationAttempt.Response.FailedPasswordAttemptCount.Should().Be((short)_maxFailedPasswordAttemptCount);
            testUserAfterFailedAuthenticationAttempt.Response.LastLockoutDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Password_Expired()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            //manually update last password change date to be past expiry threshold
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.Include(ul => ul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == testUser.UserId);
                if (entity != null)
                {
                    entity.UserLogin.LastPasswordChangeDate = DateTime.UtcNow.AddDays(-(_passwordExpiryInDays + 1));
                    await dbContext.SaveChangesAsync();
                }
            }

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedPasswordChangeRequiredErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);
            
            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion

        #region RefreshToken

        [Fact]
        public async Task RefreshToken_Should_Refresh()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token, 
                RefreshToken = authenticationResult.Response.RefreshToken }, 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().BeNullOrEmpty();
            refreshTokenResult.Response.Token.Should().NotBeNullOrEmpty();
            refreshTokenResult.Response.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_Required_Field_Errors()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenRequiredFieldErrors();

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest(), 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_Max_Length_Errors()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenMaxLengthFieldErrors();

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = "ValidToken", 
                RefreshToken = LogicTestUtilities.GenerateRandomString(2049) }, 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_Invalid_Token()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenInvalidAuthTokenErrors();

            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token + "asfasdf", //make token invalid by appending random string, 
                RefreshToken = authenticationResult.Response.RefreshToken }, 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_User_Inactive()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenUserNotFoundErrors();

            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);

            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            //deactivate user after successful authentication to set up scenario where token is valid but user is inactive
            var updateTestUserResponse = await _userLogic.Update(testUser.UserId, new InsertUpdateUserRequest {
                Active = false,
                Email = testUser.Email,
                FirstName = testUser.FirstName,
                LastName = testUser.LastName,
                DateOfBirth = testUser.DateOfBirth,
                CurrentUser = TestConstants.CurrentUser
            });

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token,
                RefreshToken = authenticationResult.Response.RefreshToken }, 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            updateTestUserResponse.Errors.Should().BeNullOrEmpty();

            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_User_Deleted()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenUserNotFoundErrors();

            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);

            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            //delete user after successful authentication to set up scenario where token is valid but user is deleted
            await _userLogic.Delete(testUser.UserId, TestConstants.CurrentUser);

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token,
                RefreshToken = authenticationResult.Response.RefreshToken }, 
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_Invalid_Refresh_Token()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenInvalidRefreshTokenErrors();

            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);

            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token,
                RefreshToken = authenticationResult.Response.RefreshToken + "asfasdf" }, //make refresh token invalid by appending random string
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        [Fact]
        public async Task RefreshToken_Should_Not_Refresh_Expired_Refresh_Token()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRefreshTokenExpiredErrors();

            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);

            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            //manually update refresh token last updated date to be past expiry threshold
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.Include(aul => aul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == testUser.UserId);
                if (entity != null)
                {
                    entity.UserLogin.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-(_refreshTokenExpiryInDays + 1));
                    await dbContext.SaveChangesAsync();
                }
            }

            // Act
            var refreshTokenResult = await _authenticationLogic.RefreshToken(new RefreshTokenRequest { 
                Token = authenticationResult.Response.Token,
                RefreshToken = authenticationResult.Response.RefreshToken },
                _userLogic, 
                _applicationLogic
            );

            // Assert
            refreshTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, refreshTokenResult.Errors);
        }

        #endregion

        #region RevokeToken

        [Fact]
        public async Task RevokeToken_Should_Revoke()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            // Act
            var revokeTokenResult = await _authenticationLogic.RevokeToken(new RevokeTokenRequest { 
                Email = testUser.Email, 
                CurrentUser = TestConstants.CurrentUser
            });

            // Assert
            revokeTokenResult.Errors.Should().BeNullOrEmpty();
            
            //verify refresh token info is nulled out on user
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.Include(aul => aul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == testUser.UserId);
                entity.UserLogin.RefreshToken.Should().BeNull();
                entity.UserLogin.RefreshTokenExpiryTime.Should().BeNull();
            }
        }

        [Fact]
        public async Task RevokeToken_Should_Not_Revoke_Required_Field_Errors()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRevokeTokenRequiredFieldErrors();

            // Act
            var revokeTokenResult = await _authenticationLogic.RevokeToken(new RevokeTokenRequest ());

            // Assert
            revokeTokenResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, revokeTokenResult.Errors);
        }

        #endregion

        #region ForgotPassword

        [Fact]
        public async Task ForgotPassword_Should_Reset_Password()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testUser = await _setupTestUserForAuthentication(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            
            var authenticationResult = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].Name, testUser.Email, testUser.Password);

            // Act
            var forgotPasswordResult = await _authenticationLogic.ForgotPassword(new ForgotPasswordRequest { 
                Email = testUser.Email, 
                CurrentUser = TestConstants.CurrentUser
            }, _userLogic);

            // Assert
            forgotPasswordResult.Errors.Should().BeNullOrEmpty();
            
            //verify password was reset on user
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.Users.Include(aul => aul.UserLogin).FirstOrDefaultAsync(ent => ent.UserId == testUser.UserId);
                //entity.Password.Should().NotBe(testUser.Password); //TODO: Decrypt password and verify it was changed once we have a way to do that in our tests
                entity.UserLogin.PasswordResetRequired.Should().BeTrue();
                entity.UserLogin.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task ForgotPassword_Should_Not_Reset_Password_Required_Field_Errors()
        {
            // Arrange
            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedForgotPasswordRequiredFieldErrors();

            // Act
            var forgotPasswordResult = await _authenticationLogic.ForgotPassword(new ForgotPasswordRequest (), _userLogic);

            // Assert
            forgotPasswordResult.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, forgotPasswordResult.Errors);
        }

        #endregion

        #region Private

        private async Task<UserDto> _setupTestUserForAuthentication(int applicationId)
        {
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            var testUser = await _userLogic.Insert(recordToCreate);
            
            //change password after initial user creation
            await _userLogic.ChangePassword(new ChangePasswordRequest {
                ApplicationUserId = testUser.Response.UserId,
                NewPassword = TestConstants.DefaultTestUserPassword,
                CurrentUser = TestConstants.CurrentUser
            });
            
            testUser.Response.Password = TestConstants.DefaultTestUserPassword;

            return testUser.Response;
        }

        private async Task<ErrorValidationResult<AuthenticationResponse>> _authenticate(string applicationName, string email, string password)
        {
            return await _authenticationLogic.Authenticate(new AuthenticationRequest { 
                ApplicationName = applicationName, 
                Email = email, 
                Password = password
            }, _userLogic, _applicationLogic);
        }

        #endregion
    
        

    }
}
