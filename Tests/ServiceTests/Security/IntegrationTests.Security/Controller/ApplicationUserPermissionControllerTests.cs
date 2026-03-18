using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
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
    public class ApplicationUserPermissionControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        public ApplicationUserPermissionControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region utils

        private async Task<ArrangeTestDataResponse> _arrangeTestData()
        {
            // Arrange
            var ret = new ArrangeTestDataResponse();
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            var inactivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);

            var activeApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
            var inactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();

            //create 5 active ApplicationUserPermission records
            foreach (var activePermission in activePermissions) 
            {
                activeApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId));
            }

            //create 5 inactive ApplicationUserPermission records
            foreach (var inactivePermission in inactivePermissions) 
            {
                inactiveApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactivePermission.PermissionId, false));
            }

            ret.ActiveApplicationUserPermissions = activeApplicationUserPermissions;
            ret.InactiveApplicationUserPermissions = inactiveApplicationUserPermissions;

            return ret;
        }

        public class ArrangeTestDataResponse 
        {
            public List<ApplicationUserPermissionDto> ActiveApplicationUserPermissions { get; set; }
            public List<ApplicationUserPermissionDto> InactiveApplicationUserPermissions { get; set; }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await _arrangeTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await _arrangeTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base);

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
            var arrangeTestDataResponse = await _arrangeTestData();
            var activeTestRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, activeTestRecord.ApplicationUserPermissionId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(result.Response, activeTestRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + inactiveTestRecord.ApplicationUserPermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + inactiveTestRecord.ApplicationUserPermissionId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var id = "asfasdfasdfasdf";

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + id);
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
            var arrangeTestDataResponse = await _arrangeTestData();
            var postReq = new FilterApplicationUserPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var postReq = new FilterApplicationUserPermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var applicationUserPermissionIds = new List<int> 
            { 
                arrangeTestDataResponse.ActiveApplicationUserPermissions[0].ApplicationUserPermissionId, 
                arrangeTestDataResponse.ActiveApplicationUserPermissions[1].ApplicationUserPermissionId,
                arrangeTestDataResponse.ActiveApplicationUserPermissions[2].ApplicationUserPermissionId 
            };
            
            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = applicationUserPermissionIds };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(3);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterApplicationUserPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/Filter", postReq);

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
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            
            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = activePermission.PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, insertReq);    
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, insertedRecordRes.Response.ApplicationUserPermissionId);

            // Assert
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(insertedRecordRes.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserPermission.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

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
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var inactivePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId, false);

            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = activePermission.PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, insertReq);  

            var updateReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = inactivePermission.PermissionId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateRecordRes = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, updateReq, insertedRecordRes.Response.ApplicationUserPermissionId);

            // Assert
            updateRecordRes.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            updateRecordRes.Response.ApplicationUserId.Should().Be(updateReq.ApplicationUserId);
            updateRecordRes.Response.PermissionId.Should().Be(updateReq.PermissionId);
            updateRecordRes.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/1", postReq);

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
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var applicationUserPermission = await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, applicationUserPermission.ApplicationUserPermissionId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + applicationUserPermission.ApplicationUserPermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applicationUserPermissionId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + applicationUserPermissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + applicationUserPermissionId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            var expectedInvalidDeleteError = _securityTestUtilities.ApplicationUserPermission.GetExpectedRecordDoesNotExistErrors();
            
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
            var applicationUserPermissionId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + applicationUserPermissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUserPermission.Base + "/" + applicationUserPermissionId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
