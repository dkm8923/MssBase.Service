using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
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
    public class ApplicationUserRoleControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        public ApplicationUserRoleControllerTests(WebApplicationFactory<Program> factory)
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
            var activeRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);
            var inactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId);

            var activeApplicationUserRoles = new List<ApplicationUserRoleDto>();
            var inactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();

            //create 5 active ApplicationUserRole records
            foreach (var activeRole in activeRoles) 
            {
                activeApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activeRole.RoleId));
            }

            //create 5 inactive ApplicationUserRole records
            foreach (var inactiveRole in inactiveRoles) 
            {
                inactiveApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactiveRole.RoleId, false));
            }

            ret.ActiveApplicationUserRoles = activeApplicationUserRoles;
            ret.InactiveApplicationUserRoles = inactiveApplicationUserRoles;

            return ret;
        }

        public class ArrangeTestDataResponse 
        {
            public List<ApplicationUserRoleDto> ActiveApplicationUserRoles { get; set; }
            public List<ApplicationUserRoleDto> InactiveApplicationUserRoles { get; set; }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await _arrangeTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base);

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
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

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
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base);

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
            var activeTestRecord = arrangeTestDataResponse.ActiveApplicationUserRoles[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, activeTestRecord.ApplicationUserRoleId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUserRole.VerifyTestRecordValuesMatch(result.Response, activeTestRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveApplicationUserRoles[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + inactiveTestRecord.ApplicationUserRoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await _arrangeTestData();
            var inactiveTestRecord = arrangeTestDataResponse.InactiveApplicationUserRoles[0];

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + inactiveTestRecord.ApplicationUserRoleId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

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
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + id);

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
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + id);
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
            var postReq = new FilterApplicationUserRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

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
            var postReq = new FilterApplicationUserRoleServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

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
            var applicationUserRoleIds = new List<int> 
            { 
                arrangeTestDataResponse.ActiveApplicationUserRoles[0].ApplicationUserRoleId, 
                arrangeTestDataResponse.ActiveApplicationUserRoles[1].ApplicationUserRoleId,
                arrangeTestDataResponse.ActiveApplicationUserRoles[2].ApplicationUserRoleId 
            };
            
            var postReq = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = applicationUserRoleIds };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(3);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterApplicationUserRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/Filter", postReq);

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
            var activeRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            
            var insertReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = activeRole.RoleId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, insertReq);    
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, insertedRecordRes.Response.ApplicationUserRoleId);

            // Assert
            _securityTestUtilities.ApplicationUserRole.VerifyTestRecordValuesMatch(insertedRecordRes.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserRole.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

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
            var activeRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var inactiveRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId, false);

            var insertReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = activeRole.RoleId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, insertReq);  

            var updateReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = inactiveRole.RoleId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateRecordRes = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, updateReq, insertedRecordRes.Response.ApplicationUserRoleId);

            // Assert
            updateRecordRes.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            updateRecordRes.Response.ApplicationUserId.Should().Be(updateReq.ApplicationUserId);
            updateRecordRes.Response.RoleId.Should().Be(updateReq.RoleId);
            updateRecordRes.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/1", null);

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
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/1", postReq);

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
            var activeRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var applicationUserRole = await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activeRole.RoleId);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.ApplicationUserRole.Base, applicationUserRole.ApplicationUserRoleId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + applicationUserRole.ApplicationUserRoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applicationUserRoleId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + applicationUserRoleId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + applicationUserRoleId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            var expectedInvalidDeleteError = _securityTestUtilities.ApplicationUserRole.GetExpectedRecordDoesNotExistErrors();
            
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
            var applicationUserRoleId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + applicationUserRoleId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUserRole.Base + "/" + applicationUserRoleId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
