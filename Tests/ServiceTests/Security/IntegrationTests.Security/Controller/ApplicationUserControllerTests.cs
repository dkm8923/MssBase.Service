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
using Dto.Security.Application;
using IntegrationTests.Shared.Models;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserControllerTests : SecurityTestBase, 
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
        private readonly string _defaultApplicationUserApiEndPoint = ApiEndPoints.Security.ApplicationUser.Base;

        public ApplicationUserControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(3);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserPermissions.Should().BeNull();
                applicationUser.ApplicationUserRoles.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint, 
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultApplicationUserApiEndPoint, invalidToken);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());
            
            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultApplicationUserApiEndPoint, token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();

            _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: false);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
            _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: true);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = testRecord.ApplicationUserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.ApplicationUserPermissions.Should().BeNull();
            result.Response.ApplicationUserRoles.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";
           
            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationUserApiEndPoint + "/" + id);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getByIdRequest, token);
            
            // Act
            var getResponse = await _client.SendAsync(getByIdRequest);
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserApiEndPoint, 1, invalidToken);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserApiEndPoint, 1, token);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationUserServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationUserServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0); //activeRecords
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0); //inactiveRecords
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUsers = arrangeTestDataResponse.ActiveApplicationUsers;
            
            var postReq = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUsers[0].ApplicationUserId, applicationUsers[1].ApplicationUserId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            var postReqInvalidCreatedBy = new FilterApplicationUserServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationUserServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidApplicationUserIds = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { 9999 } };
            var postReqInvalidEmail = new FilterApplicationUserServiceRequest { Email = "invalidemail@test.com" };
            var postReqInvalidFirstName = new FilterApplicationUserServiceRequest { FirstName = "InvalidFirstName" };
            var postReqInvalidLastName = new FilterApplicationUserServiceRequest { LastName = "InvalidLastName" };
            var postReqInvalidDateOfBirth = new FilterApplicationUserServiceRequest { DateOfBirth = LogicTestUtilities.GetRandomDateTime(1999) };
            var postReqInvalidApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = 9999 };

            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidApplicationUserIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationUserIds });
            var invalidEmailResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidEmail });
            var invalidFirstNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidFirstName });
            var invalidLastNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidLastName });
            var invalidDateOfBirthResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidDateOfBirth });
            var invalidApplicationIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationUserApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationId });

            //Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdsResult.Response.Should().HaveCount(0);
            invalidEmailResult.Response.Should().HaveCount(0);
            invalidFirstNameResult.Response.Should().HaveCount(0);
            invalidLastNameResult.Response.Should().HaveCount(0);
            invalidDateOfBirthResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = arrangeTestDataResponse.ActiveApplicationUsers[0].ApplicationUserId;
            
            var postReq = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUserId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = arrangeTestDataResponse.InactiveApplicationUsers[0].ApplicationUserId;
            
            var postReq = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUserId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = arrangeTestDataResponse.ActiveApplicationUsers[0].ApplicationUserId;
            
            var postReq = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUserId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationUserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserPermissions.Should().BeNull();
                applicationUser.ApplicationUserRoles.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserApiEndPoint,"", token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserApiEndPoint, new FilterApplicationUserServiceRequest(), invalidToken);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());

            // Act
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationUserApiEndPoint, new FilterApplicationUserServiceRequest(), token);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationUserDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationUserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = insertResult.Response.ApplicationUserId,
                Token = token
            });
            
            //Assert
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserApiEndPoint, "", token);
            
            //assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserApiEndPoint, new InsertUpdateApplicationRequest(), invalidToken);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationUserApiEndPoint, new InsertUpdateApplicationRequest(), token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var insertedRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = insertedRecord.ApplicationId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationUserDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationUserApiEndPoint,
                RecordId = insertedRecord.ApplicationUserId,
                Token = token, 
                RequestObject = updateReq
            });
           
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserApiEndPoint, "", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserApiEndPoint, new InsertUpdateApplicationRequest(), 1, invalidToken);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationUserApiEndPoint, new InsertUpdateApplicationRequest(), 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers[0];

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserApiEndPoint,testRecord.ApplicationUserId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserApiEndPoint,testRecord.ApplicationUserId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationUserApiEndPoint, applicationUserId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserApiEndPoint, applicationUserId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationUserApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultApplicationUserApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(deleteRequest, token);

            // Act
            var getResponse = await _client.SendAsync(getRequest);
            var deleteResponse = await _client.SendAsync(deleteRequest);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Delete_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserApiEndPoint, 1, invalidToken);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Delete_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0], new AssignRoleRequest());

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationUserApiEndPoint, 1, token);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationUserReadOnly = true });
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
