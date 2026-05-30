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
using Dto.Security.Application;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserPermissionControllerTests : SecurityTestBase, 
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

        public ApplicationUserPermissionControllerTests(WebApplicationFactory<Program> factory)
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
            await ArrangeApplicationUserPermissionTestData();

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
            await ArrangeApplicationUserPermissionTestData();

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

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Should().NotBeNull();
                applicationUserPermission.Active.Should().BeTrue();
                applicationUserPermission.Permission.Should().NotBeNull();
                applicationUserPermission.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true) + "&" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().BeNull();
            }
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, testRecord.ApplicationUserPermissionId, new BaseServiceGet { IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();
            result.Response.Permission.Active.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, testRecord.ApplicationUserPermissionId, new BaseServiceGet { IncludeInactive = true, IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, testRecord.ApplicationUserPermissionId, new BaseServiceGet { IncludeRelated = false });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().BeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
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
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var applicationUserPermissions = arrangeTestDataResponse.ActiveApplicationUserPermissions.Take(5).ToList();
            
            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { applicationUserPermissions[0].ApplicationUserPermissionId, applicationUserPermissions[1].ApplicationUserPermissionId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
                r.Permission.Should().NotBeNull();
                r.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var activeApplicationUserPermissions = arrangeTestDataResponse.ActiveApplicationUserPermissions.Take(5).ToList();
            var inactiveApplicationUserPermissions = arrangeTestDataResponse.InactiveApplicationUserPermissions.Take(5).ToList();

            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { activeApplicationUserPermissions[0].ApplicationUserPermissionId, activeApplicationUserPermissions[1].ApplicationUserPermissionId, inactiveApplicationUserPermissions[0].ApplicationUserPermissionId, inactiveApplicationUserPermissions[1].ApplicationUserPermissionId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(4);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var postReq = new FilterApplicationUserPermissionServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(_client, ApiEndPoints.Security.ApplicationUserPermission.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().BeNull();
            }
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

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserPermissionAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserPermissionReadOnly = true });
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
