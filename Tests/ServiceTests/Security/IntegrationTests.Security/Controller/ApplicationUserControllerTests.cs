using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Models;
using System.Net;
using IntegrationTests.Shared.Utilities;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserControllerTests : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApplicationUserControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId, 1, false);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId, 1, false);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base + "?" + ControllerTestUtilities.createIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task ApplicationUser_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.DeleteAllRecords();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
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
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, ApiEndPoints.Security.ApplicationUser.Base, testRecord.ApplicationUserId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task ApplicationUser_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ApplicationUser_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId + "?" + ControllerTestUtilities.createIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ApplicationUser_GetById_Should_Return_NotFound()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task ApplicationUser_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var id = "asfasdfasdfasdf";

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + id);
            var content = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Errors.Count.Should().Be(1);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);

            var postReq = new FilterApplicationUserServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId, 1, false);

            var postReq = new FilterApplicationUserServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base + "/Filter", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task ApplicationUser_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(null);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task ApplicationUser_Insert_Should_Create_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var insertedRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, ApiEndPoints.Security.ApplicationUser.Base, insertedRecord.ApplicationUserId);

            // Assert
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task ApplicationUser_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base, null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task ApplicationUser_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base, postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task ApplicationUser_Update_Should_Update_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertedRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = applicationId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationUserDto>(_client, ApiEndPoints.Security.ApplicationUser.Base, updateReq, insertedRecord.ApplicationUserId);

            // Assert
            updateResult.Response.ApplicationUserId.Should().Be(insertedRecord.ApplicationUserId);
            updateResult.Response.Email.Should().Be(updateReq.Email);
            updateResult.Response.FirstName.Should().Be(updateReq.FirstName);
            updateResult.Response.LastName.Should().Be(updateReq.LastName);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
        }

        [Fact]
        public async Task ApplicationUser_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUser.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task ApplicationUser_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUser.Base + "/1", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task ApplicationUser_Delete_Should_Delete_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.ApplicationUser.Base, testRecord.ApplicationUserId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
