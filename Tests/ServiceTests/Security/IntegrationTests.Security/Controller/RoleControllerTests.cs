using Dto.Security.Role;
using Dto.Security.Role.Service;
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
    public class RoleControllerTests : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RoleControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Role_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateActiveTestRecords(roleId);
            await _securityTestUtilities.Role.CreateInactiveTestRecords(roleId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Role_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateActiveTestRecords(roleId);
            await _securityTestUtilities.Role.CreateInactiveTestRecords(roleId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Role_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.Role.DeleteAllRecords();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Role_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Role_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Role_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Role_GetById_Should_Return_NotFound()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateActiveTestRecords(roleId);
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task Role_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var id = "asfasdfasdfasdf";

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + id);
            var content = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Errors.Count.Should().Be(1);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Role_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateActiveTestRecords(roleId);

            var postReq = new FilterRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task Role_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateActiveTestRecords(roleId);
            await _securityTestUtilities.Role.CreateInactiveTestRecords(roleId, 1);

            var postReq = new FilterRoleServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Role_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base + "/Filter", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Role_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(null);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Role_Insert_Should_Create_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var insertedRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, insertedRecord.RoleId);

            // Assert
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Role_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base, null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Role_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base, postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Role_Update_Should_Update_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertedRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId);

            var updateReq = new InsertUpdateRoleRequest
            {
                Name = "test name updated",
                Description = "Test Description Updated",
                Active = false,
                ApplicationId = roleId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, updateReq, insertedRecord.RoleId);

            // Assert
            updateResult.Response.RoleId.Should().Be(insertedRecord.RoleId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
        }

        [Fact]
        public async Task Role_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Role.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Role_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Role.Base + "/1", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Role_Delete_Should_Delete_Record()
        {
            // Arrange
            int roleId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(roleId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
