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
using IntegrationTests.Shared.Models;

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
        private readonly string _defaultApplicationUserPermissionApiEndPoint = ApiEndPoints.Security.ApplicationUserPermission.Base;

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint, 
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(10);

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.NotFound
            });
            
            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = id,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.NotFound
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }
 
        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";
           
            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationUserPermissionApiEndPoint + "/" + id);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getByIdRequest, token);
            
            // Act
            var getResponse = await _client.SendAsync(getByIdRequest);
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = testRecord.ApplicationUserPermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationUserPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationUserPermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserPermissionIds = new List<int> 
            { 
                arrangeTestDataResponse.ActiveApplicationUserPermissions[0].ApplicationUserPermissionId, 
                arrangeTestDataResponse.ActiveApplicationUserPermissions[1].ApplicationUserPermissionId,
                arrangeTestDataResponse.ActiveApplicationUserPermissions[2].ApplicationUserPermissionId 
            };
            
            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = applicationUserPermissionIds };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Response.Should().HaveCount(3);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            var postReqInvalidCreatedBy = new FilterApplicationUserPermissionServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserPermissionServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationUserPermissionServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserPermissionServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidApplicationUserPermissionIds = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { 9999 } };
            var postReqInvalidApplicationId = new FilterApplicationUserPermissionServiceRequest { ApplicationId = 9999 };
            var postReqInvalidApplicationUserIds = new FilterApplicationUserPermissionServiceRequest { ApplicationUserId = 9999 };
            var postReqInvalidPermissionIds = new FilterApplicationUserPermissionServiceRequest { PermissionId = 9999 };

            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidApplicationUserPermissionIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationUserPermissionIds });
            var invalidApplicationIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationId });
            var invalidApplicationUserIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationUserIds });
            var invalidPermissionIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidPermissionIds });

            //Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserPermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdsResult.Response.Should().HaveCount(0);
            invalidPermissionIdsResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserPermissions = arrangeTestDataResponse.ActiveApplicationUserPermissions.Take(5).ToList();
            
            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { applicationUserPermissions[0].ApplicationUserPermissionId, applicationUserPermissions[1].ApplicationUserPermissionId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var activeApplicationUserPermissions = arrangeTestDataResponse.ActiveApplicationUserPermissions.Take(5).ToList();
            var inactiveApplicationUserPermissions = arrangeTestDataResponse.InactiveApplicationUserPermissions.Take(5).ToList();

            var postReq = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { activeApplicationUserPermissions[0].ApplicationUserPermissionId, activeApplicationUserPermissions[1].ApplicationUserPermissionId, inactiveApplicationUserPermissions[0].ApplicationUserPermissionId, inactiveApplicationUserPermissions[1].ApplicationUserPermissionId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

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
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationUserPermissionServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserPermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().BeNull();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserPermissionApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserPermissionApiEndPoint,"", token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(application);
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
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserPermissionDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserPermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = insertResult.Response.ApplicationUserPermissionId,
                Token = token
            });

            // Assert
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserPermissionApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserPermissionApiEndPoint, "", token);
            
            //assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(application);
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

            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserPermissionDto>(new HttpPostRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                Token = token,
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });

            var updateReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = inactivePermission.PermissionId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateRecordRes = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationUserPermissionDto>(new HttpPutRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserPermissionApiEndPoint,
                RecordId = insertedRecordRes.Response.ApplicationUserPermissionId,
                Token = token,
                RequestObject = updateReq
            });

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserPermissionApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserPermissionApiEndPoint, "", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions[0];

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserPermissionApiEndPoint, testRecord.ApplicationUserPermissionId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserPermissionApiEndPoint, testRecord.ApplicationUserPermissionId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserPermissionApiEndPoint, applicationUserId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserPermissionApiEndPoint, applicationUserId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.ApplicationUserPermission.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationUserPermissionApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultApplicationUserPermissionApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(deleteRequest, token);

            // Act
            var getResponse = await _client.SendAsync(getRequest);
            var deleteResponse = await _client.SendAsync(deleteRequest);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

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
