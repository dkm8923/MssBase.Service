using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
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
    public class ApplicationUserControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        public ApplicationUserControllerTests(WebApplicationFactory<Program> factory)
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
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base);

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
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, 1);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

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
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);
            result.Response.FirstOrDefault().ApplicationUserPermissions.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(false));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);
            result.Response.FirstOrDefault().ApplicationUserPermissions.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, ApiEndPoints.Security.ApplicationUser.Base, testRecord.ApplicationUserId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
 
        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
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

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, 
                                                                                                            ApiEndPoints.Security.ApplicationUser.Base, 
                                                                                                            applicationUser.FirstOrDefault().ApplicationUserId, 
                                                                                                            new BaseServiceGet { IncludeRelated = true });
            
            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            result.Response.ApplicationUserPermissions.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, 
                                                                                                            ApiEndPoints.Security.ApplicationUser.Base, 
                                                                                                            applicationUser.FirstOrDefault().ApplicationUserId, 
                                                                                                            new BaseServiceGet { IncludeRelated = false });
            
            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            result.Response.ApplicationUserPermissions.Should().HaveCount(0);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);

            var postReq = new FilterApplicationUserServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

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
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, 1);

            var postReq = new FilterApplicationUserServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

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
            var applicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 5);
            
            var postReq = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUsers[0].ApplicationUserId, applicationUsers[1].ApplicationUserId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(2);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterApplicationUserServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            var postReq = new FilterApplicationUserServiceRequest { IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            //Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.FirstOrDefault().ApplicationUserPermissions.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            
            foreach (var permission in permissions)
            {
                await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.FirstOrDefault().ApplicationUserId, permission.PermissionId, 1);
            }

            var postReq = new FilterApplicationUserServiceRequest { IncludeRelated = false };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(_client, ApiEndPoints.Security.ApplicationUser.Base, postReq);

            //Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.FirstOrDefault().ApplicationUserPermissions.Should().HaveCount(0);
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
            var insertedRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(_client, ApiEndPoints.Security.ApplicationUser.Base, insertedRecord.ApplicationUserId);

            // Assert
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.ApplicationUser.Base, postReq);

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
            var insertedRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = application.ApplicationId,
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
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUser.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            int applicationUserId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.ApplicationUser.Base + "/1", postReq);

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
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.ApplicationUser.Base, testRecord.ApplicationUserId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + testRecord.ApplicationUserId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applicationUserId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + applicationUserId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + applicationUserId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            //TODO: Use hardcoded string for testing. (Should be in application utilities)
            var expectedInvalidDeleteError = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();
            
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
            var applicationUserId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + applicationUserId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.ApplicationUser.Base + "/" + applicationUserId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
