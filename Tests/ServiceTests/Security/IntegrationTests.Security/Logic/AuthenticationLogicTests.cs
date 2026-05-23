using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Dto.Security.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class AuthenticationLogicTests : SecurityTestBase
    {
        private int _maxFailedPasswordAttemptCount => _authenticationSettingsConfigMonitor.CurrentValue.MaxFailedPasswordAttemptCount;
        private int _passwordExpiryInDays => _authenticationSettingsConfigMonitor.CurrentValue.PasswordExpiryInDays;

        #region Authenticate

        [Fact]
        public async Task Authenticate_Should_Authenticate_User()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            var newPassword = TestConstants.DefaultNewPassword;
            
            //change password after initial user creation
            await _applicationUserLogic.ChangePassword(new ChangePasswordRequest {
                ApplicationUserId = testUser.Response.ApplicationUserId,
                NewPassword = newPassword,
                CurrentUser = TestConstants.CurrentUser
            });

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, newPassword);

            var testUserAfterSuccessfulAuthentication = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Token.Should().NotBeNullOrEmpty();
            testUserAfterSuccessfulAuthentication.Response.FailedPasswordAttemptCount.Should().Be(0);
            testUserAfterSuccessfulAuthentication.Response.LastPasswordChangeDate.Should().NotBeNull();
            testUserAfterSuccessfulAuthentication.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            testUserAfterSuccessfulAuthentication.Response.LastLoginDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_Email()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            
            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, "InvalidEmail@example.com", testUser.Response.Password);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidCredentialsErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_Password()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            
            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, "InvalidPassword");

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidCredentialsErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Invalid_ApplicationId()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            
            // Act
            var result = await _authenticate(99, testUser.Response.Email, testUser.Response.Password);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedInvalidApplicationIdFieldErrors();

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Password_Reset_Required()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedPasswordChangeRequiredErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, testUser.Response.Password);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, null, null);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Field_Max_Length_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, LogicTestUtilities.GenerateRandomString(120) + "@test.com", LogicTestUtilities.GenerateRandomString(65));

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Authenticate_Should_Not_Authenticate_User_Account_Locked()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            var newPassword = TestConstants.DefaultNewPassword;
            
            //change password after initial user creation
            await _applicationUserLogic.ChangePassword(new ChangePasswordRequest {
                ApplicationUserId = testUser.Response.ApplicationUserId,
                NewPassword = newPassword,
                CurrentUser = TestConstants.CurrentUser
            });

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedAccountLockedErrors();

            //attempt to authenticate with invalid password until account is locked
            for (int i = 0; i < _maxFailedPasswordAttemptCount; i++)
            {                
                await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, "InvalidPassword");
            }

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, newPassword);
            
            var testUserAfterFailedAuthenticationAttempt = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            var newPassword = TestConstants.DefaultNewPassword;
            
            //change password after initial user creation
            await _applicationUserLogic.ChangePassword(new ChangePasswordRequest {
                ApplicationUserId = testUser.Response.ApplicationUserId,
                NewPassword = newPassword,
                CurrentUser = TestConstants.CurrentUser
            });

            //manually update last password change date to be past expiry threshold
            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                var entity = await dbContext.ApplicationUsers.FirstOrDefaultAsync(ent => ent.ApplicationUserId == testUser.Response.ApplicationUserId);
                if (entity != null)
                {
                    entity.LastPasswordChangeDate = DateTime.UtcNow.AddDays(-(_passwordExpiryInDays + 1));
                    await dbContext.SaveChangesAsync();
                }
            }

            var expectedFieldErrors = _securityTestUtilities.Authentication.GetExpectedPasswordChangeRequiredErrors();

            // Act
            var result = await _authenticate(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, testUser.Response.Email, newPassword);
            
            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #region Private

        private async Task<ErrorValidationResult<AuthenticationResponse>> _authenticate(int applicationId, string email, string password)
        {
            return await _authenticationLogic.Authenticate(new AuthenticationRequest { 
                ApplicationId = applicationId, 
                Email = email, 
                Password = password
            }, _applicationUserLogic, _applicationLogic);
        }

        #endregion

        #endregion

        

        
    
        

    }
}
