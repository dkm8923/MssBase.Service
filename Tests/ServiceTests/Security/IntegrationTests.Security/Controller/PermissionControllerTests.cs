using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Models;
using System.Net;
using IntegrationTests.Shared.Utilities;
using IntegrationTests.Shared.Utilities.Contracts.Controller;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class PermissionControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        public PermissionControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(_client, ApiEndPoints.Security.Permission.Base, testRecord.PermissionId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
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
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);

            var postReq = new FilterPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

            var postReq = new FilterPermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 5);
            
            var postReq = new FilterPermissionServiceRequest { PermissionIds = new List<int> { permissions[0].PermissionId, permissions[1].PermissionId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(2);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(_client, ApiEndPoints.Security.Permission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base + "/Filter", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var postReq = ControllerTestUtilities.FormatPostRequest(null);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            
            // Act
            var insertedRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(_client, ApiEndPoints.Security.Permission.Base, insertedRecord.PermissionId);

            // Assert
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base, null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Permission.Base, postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var insertedRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "name update",
                Description = "description update",
                Active = false,
                ApplicationId = application.ApplicationId,
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
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Permission.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
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
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Permission.Base, testRecord.PermissionId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + testRecord.PermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var permissionId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + permissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Permission.Base + "/" + permissionId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            //TODO: Use hardcoded string for testing. (Should be in application utilities)
            var expectedInvalidDeleteError = _securityTestUtilities.Permission.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var permissionId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Permission.Base + "/" + permissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Permission.Base + "/" + permissionId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
