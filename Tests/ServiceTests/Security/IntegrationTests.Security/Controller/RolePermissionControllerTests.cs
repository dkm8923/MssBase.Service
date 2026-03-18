using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
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
    public class RolePermissionControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        public RolePermissionControllerTests(WebApplicationFactory<Program> factory)
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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            var inactivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);

            var activeRolePermissions = new List<RolePermissionDto>();
            var inactiveRolePermissions = new List<RolePermissionDto>();

            //create 5 active RolePermission records
            foreach (var activePermission in activePermissions) 
            {
                activeRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, activePermission.PermissionId));
            }

            //create 5 inactive RolePermission records
            foreach (var inactivePermission in inactivePermissions) 
            {
                inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, inactivePermission.PermissionId, false));
            }

            ret.ActiveRolePermissions = activeRolePermissions;
            ret.InactiveRolePermissions = inactiveRolePermissions;

            return ret;
        }

        public class ArrangeTestDataResponse 
        {
            public List<RolePermissionDto> ActiveRolePermissions { get; set; }
            public List<RolePermissionDto> InactiveRolePermissions { get; set; }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await _arrangeTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base);

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
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

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
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base);

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
            var activeTestRecord = arrangeTestDataResponse.ActiveRolePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(_client, ApiEndPoints.Security.RolePermission.Base, activeTestRecord.RolePermissionId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(result.Response, activeTestRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveRolePermissions[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + inactiveTestRecord.RolePermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveRolePermissions[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + inactiveTestRecord.RolePermissionId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

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
            var response = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + id);

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
            var response = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + id);
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
            var postReq = new FilterRolePermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base, postReq);

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
            var postReq = new FilterRolePermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base, postReq);

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
            var rolePermissionIds = new List<int> 
            { 
                arrangeTestDataResponse.ActiveRolePermissions[0].RolePermissionId, 
                arrangeTestDataResponse.ActiveRolePermissions[1].RolePermissionId,
                arrangeTestDataResponse.ActiveRolePermissions[2].RolePermissionId 
            };
            
            var postReq = new FilterRolePermissionServiceRequest { RolePermissionIds = rolePermissionIds };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(3);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterRolePermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(_client, ApiEndPoints.Security.RolePermission.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.RolePermission.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.RolePermission.Base + "/Filter", postReq);

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            
            var insertReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = activePermission.PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<RolePermissionDto>(_client, ApiEndPoints.Security.RolePermission.Base, insertReq);    
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(_client, ApiEndPoints.Security.RolePermission.Base, insertedRecordRes.Response.RolePermissionId);

            // Assert
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(insertedRecordRes.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.RolePermission.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.RolePermission.Base, postReq);

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var inactivePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId, false);

            var insertReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = activePermission.PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<RolePermissionDto>(_client, ApiEndPoints.Security.RolePermission.Base, insertReq);  

            var updateReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = inactivePermission.PermissionId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateRecordRes = await ControllerTestUtilities.UpdateRecordWithValidationResult<RolePermissionDto>(_client, ApiEndPoints.Security.RolePermission.Base, updateReq, insertedRecordRes.Response.RolePermissionId);

            // Assert
            updateRecordRes.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            updateRecordRes.Response.RoleId.Should().Be(updateReq.RoleId);
            updateRecordRes.Response.PermissionId.Should().Be(updateReq.PermissionId);
            updateRecordRes.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.RolePermission.Base + "/1", null);

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
            var response = await _client.PutAsync(ApiEndPoints.Security.RolePermission.Base + "/1", postReq);

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var activePermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var rolePermission = await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, activePermission.PermissionId);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.RolePermission.Base, rolePermission.RolePermissionId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + rolePermission.RolePermissionId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var rolePermissionId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + rolePermissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.RolePermission.Base + "/" + rolePermissionId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            var expectedInvalidDeleteError = _securityTestUtilities.RolePermission.GetExpectedRecordDoesNotExistErrors();
            
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
            var rolePermissionId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.RolePermission.Base + "/" + rolePermissionId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.RolePermission.Base + "/" + rolePermissionId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
