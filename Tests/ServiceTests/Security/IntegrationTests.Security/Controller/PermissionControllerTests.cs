using Dto.Security.Permission;
using Dto.Security.Permission.Service;
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
    public class PermissionControllerTests : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PermissionControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Permission_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId, 1, false);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Permission_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId, 1, false);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Permission_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.Permission.DeleteAllRecords();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Permission_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(_client, ApiEndPoints.Security.Permission.Base, testRecord.PermissionId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Permission_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Permission_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Permission_GetById_Should_Return_NotFound()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId);
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task Permission_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var id = "asfasdfasdfasdf";

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + id);
            var content = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Errors.Count.Should().Be(1);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Permission_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId);

            var postReq = new FilterPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task Permission_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId);
            await _securityTestUtilities.Permission.CreateTestRecords(permissionId, 1, false);

            var postReq = new FilterPermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Permission_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base + "/Filter", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Permission_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(null);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Permission_Insert_Should_Create_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var insertedRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(_client, ApiEndPoints.Security.Permission.Base, insertedRecord.PermissionId);

            // Assert
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Permission_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base, null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Permission_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base, postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Permission_Update_Should_Update_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertedRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId);

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "test name updated",
                Description = "Test Description Updated",
                Active = false,
                ApplicationId = permissionId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<PermissionDto>(_client, ApiEndPoints.Security.Permission.Base, updateReq, insertedRecord.PermissionId);

            // Assert
            updateResult.Response.PermissionId.Should().Be(insertedRecord.PermissionId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
        }

        [Fact]
        public async Task Permission_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Permission.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Permission_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Permission.Base + "/1", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Permission_Delete_Should_Delete_Record()
        {
            // Arrange
            int permissionId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(permissionId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Permission.Base, testRecord.PermissionId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
