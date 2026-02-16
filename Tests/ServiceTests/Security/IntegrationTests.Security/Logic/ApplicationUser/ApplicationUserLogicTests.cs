using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;

namespace IntegrationTests.Security.Logic.ApplicationUser
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserLogicTests : SecurityTestBase
    {
        #region GetAll

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.DeleteAllRecords();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task ApplicationUser_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task ApplicationUser_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task ApplicationUser_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);

            var postReq = new FilterApplicationUserLogicRequest { };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            var postReq = new FilterApplicationUserLogicRequest { IncludeInactive = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);

            var postReqInvalidEmail = new FilterApplicationUserServiceRequest { Email = "invalid@test.com" };
            var postReqInvalidApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = -1 };
            
            // Act
            var invalidEmailResult = await _applicationUserLogic.Filter(postReqInvalidEmail);
            var invalidApplicationIdResult = await _applicationUserLogic.Filter(postReqInvalidApplicationId);
            
            // Assert
            invalidEmailResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task ApplicationUser_Insert_Should_Create_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(applicationId);

            // Act
            var result = await _applicationUserLogic.Insert(insertReq);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.Email.Should().Be(insertReq.Email);
        }

        #endregion

        #region Update

        [Fact]
        public async Task ApplicationUser_Update_Should_Update_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = applicationId,
                CurrentUser = "IntegrationTest"
            };

            // Act
            var result = await _applicationUserLogic.Update(testRecord.ApplicationUserId, updateReq);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Email.Should().Be(updateReq.Email);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task ApplicationUser_Delete_Should_Delete_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            // Act
            var result = await _applicationUserLogic.Delete(testRecord.ApplicationUserId);
            var getResult = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        #endregion
    }
}
