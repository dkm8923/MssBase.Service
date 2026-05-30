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
using Dto.Security.Application;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserRoleControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetAllIncludeRelated,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsGetByIdIncludeRelated,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsFilterIncludeRelated, 
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

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await ArrangeApplicationUserRoleTestData();

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
            await ArrangeApplicationUserRoleTestData();

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

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var applicationUserRole in result.Response)
            {
                applicationUserRole.Should().NotBeNull();
                applicationUserRole.Active.Should().BeTrue();
                applicationUserRole.Role.Should().NotBeNull();
                applicationUserRole.Role.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true) + "&" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);

            foreach (var applicationUserRole in result.Response)
            {
                applicationUserRole.Role.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var applicationUserRole in result.Response)
            {
                applicationUserRole.Role.Should().BeNull();
            }
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, testRecord.ApplicationUserRoleId, new BaseServiceGet { IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Role.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();
            result.Response.Role.Active.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, testRecord.ApplicationUserRoleId, new BaseServiceGet { IncludeInactive = true, IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Role.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserRoleDto>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, testRecord.ApplicationUserRoleId, new BaseServiceGet { IncludeRelated = false });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Role.Should().BeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
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
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var applicationUserRoles = arrangeTestDataResponse.ActiveApplicationUserRoles.Take(5).ToList();
            
            var postReq = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = new List<int> { applicationUserRoles[0].ApplicationUserRoleId, applicationUserRoles[1].ApplicationUserRoleId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
                r.Role.Should().NotBeNull();
                r.Role.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var activeApplicationUserRoles = arrangeTestDataResponse.ActiveApplicationUserRoles.Take(5).ToList();
            var inactiveApplicationUserRoles = arrangeTestDataResponse.InactiveApplicationUserRoles.Take(5).ToList();

            var postReq = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = new List<int> { activeApplicationUserRoles[0].ApplicationUserRoleId, activeApplicationUserRoles[1].ApplicationUserRoleId, inactiveApplicationUserRoles[0].ApplicationUserRoleId, inactiveApplicationUserRoles[1].ApplicationUserRoleId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(4);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Role.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserRoleTestData();
            var postReq = new FilterApplicationUserRoleServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserRoleDto>>(_client, ApiEndPoints.Security.ApplicationUserRole.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Role.Should().BeNull();
            }
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

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserRoleAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserRoleReadOnly = true });
        }

        private async Task<string> CreateAuthenticatedTestUserAndReturnToken(ApplicationDto application, AssignRoleRequest assignRoleRequest)
        {
            //authenticate test user and receive token to be used in authorized controller calls
            var testUser = await CreateTestUserWithPermissions(application.ApplicationId, assignRoleRequest);
            var authResult = await ControllerTestUtilities.AuthenticateTestUserAndReturnAuthToken(_client, testUser.Email, TestConstants.DefaultTestUserPassword, application.Name);
            
            return authResult.Token;
        }

        #endregion
    }
}
