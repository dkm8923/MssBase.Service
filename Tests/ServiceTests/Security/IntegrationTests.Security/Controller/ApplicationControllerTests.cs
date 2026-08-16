using Dto.Security.Application;
using Dto.Security.Application.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Models;
using System.Net;
using IntegrationTests.Shared.Utilities;
using IntegrationTests.Shared.Utilities.Contracts.Controller;
using IntegrationTests.Shared.Models;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationControllerTests : SecurityTestBase, 
                                              IClassFixture<WebApplicationFactory<Program>>,
                                              IDefaultControllerTestsGetAll,
                                              IDefaultControllerTestsGetAllIncludeRelated,
                                              IDefaultLogicTestsGetAllReadOnly,
                                              IDefaultControllerTestsGetById,
                                              IDefaultControllerTestsGetByIdIncludeRelated,
                                              IDefaultLogicTestsGetByIdReadOnly,
                                              IDefaultControllerTestsFilter,
                                              IDefaultControllerTestsFilterIncludeRelated,
                                              IDefaultLogicTestsFilterReadOnly,
                                              IDefaultControllerTestsInsert,
                                              IDefaultControllerTestsUpdate,
                                              IDefaultLogicTestsUpdateReadOnly,
                                              IDefaultControllerTestsDelete,
                                              IDefaultLogicTestsDeleteReadOnly
    {
        private readonly HttpClient _client;
        private readonly string _defaultApplicationApiEndPoint = ApiEndPoints.Security.Application.Base;

        public ApplicationControllerTests(WebApplicationFactory<Program> factory)
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultApplicationApiEndPoint, invalidToken);

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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultApplicationApiEndPoint, token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var application in result.Response)
            {
                _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(application, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var application in result.Response)
            {
                _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(application, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var application in result.Response)
            {
                application.ApplicationUsers.Should().BeNull();
                application.Permissions.Should().BeNull();
                application.Roles.Should().BeNull();
                application.RolePermissions.Should().BeNull();
                application.ApplicationUserPermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeReadOnly = true, DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            var readOnlyRecordCt = result.Response.Count(x => x.ReadOnly);
            readOnlyRecordCt.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeReadOnly = true, IncludeInactive = true, DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            var readOnlyInactiveRecordCt = result.Response.Count(x => x.ReadOnly && !x.Active);
            readOnlyInactiveRecordCt.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_ReadOnly_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true } 
            });

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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications[0];

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplications[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.NotFound,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });
            
            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplications[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = id,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.NotFound,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";

            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationApiEndPoint + "/" + id);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getByIdRequest, token);
            
            // Act
            var getResponse = await _client.SendAsync(getByIdRequest);
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = arrangeTestDataResponse.ActiveApplications[0].ApplicationId;
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = id,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.OK,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.ApplicationUsers.Should().HaveCountGreaterThan(0);
            result.Response.Permissions.Should().HaveCountGreaterThan(0);
            result.Response.Roles.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = arrangeTestDataResponse.ActiveApplications[0].ApplicationId;
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = id,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.OK,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.ApplicationUsers.Should().BeNull();
            result.Response.Permissions.Should().BeNull();
            result.Response.Roles.Should().BeNull();
            result.Response.RolePermissions.Should().BeNull();
            result.Response.ApplicationUserPermissions.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationApiEndPoint, 1, invalidToken);

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
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationApiEndPoint, 1, token);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();

            _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(result.Response, includeInactive: false);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplications.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
            _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(result.Response, includeInactive: true);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeTrue();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplications.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeFalse();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = testRecord.ApplicationId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true },
                ExpectedStatusCode = System.Net.HttpStatusCode.NotFound
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationServiceRequest { DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationServiceRequest { IncludeInactive = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser, DeleteCache = true };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow), DeleteCache = true };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = TestConstants.CurrentUser, DeleteCache = true };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow), DeleteCache = true };
            var postReqName = new FilterApplicationServiceRequest { Name = testRecord.Name, DeleteCache = true };
            
            // Act
            var filterCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqCreatedBy });
            var filterCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqCreatedOnDate });
            var filterUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqUpdatedBy });
            var filterUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqUpdatedOnDate });
            var filterNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqName });
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCountGreaterThan(0);
            filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedByResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterNameResult.Response.Should().HaveCount(1);
            filterNameResult.Response.First().Name.Should().Be(postReqName.Name);

            foreach (var record in filterCreatedByResult.Response)
            {
                record.CreatedBy.Should().Be(postReqCreatedBy.CreatedBy);
            }

            foreach (var record in filterCreatedOnDateResult.Response)
            {
                DateOnly.FromDateTime((DateTime)record.CreatedOn).Should().Be(postReqCreatedOnDate.CreatedOnDate);
            }

            foreach (var record in filterUpdatedByResult.Response)
            {   
                record.UpdatedBy.Should().Be(postReqUpdatedBy.UpdatedBy);
            }

            foreach (var record in filterUpdatedOnDateResult.Response)
            {
                DateOnly.FromDateTime((DateTime)record.UpdatedOn).Should().Be(postReqUpdatedOnDate.UpdatedOnDate);
            }

            foreach (var record in filterNameResult.Response)
            {
                record.Name.Should().Be(postReqName.Name);
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReqInvalidCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "TestCreatedBy", DeleteCache = true };
            var postReqInvalidCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000"), DeleteCache = true };
            var postReqInvalidUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "TestUpdatedBy", DeleteCache = true };
            var postReqInvalidUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000"), DeleteCache = true };
            var postReqInvalidName = new FilterApplicationServiceRequest { Name = "asdfasfasdfsd", DeleteCache = true };
            
            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint,Token = token, RequestObject = postReqInvalidName });
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidNameResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationApiEndPoint,"", token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = arrangeTestDataResponse.ActiveApplications[0].ApplicationId;

            var postReq = new FilterApplicationServiceRequest { IncludeRelated = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token, 
                RequestObject = postReq
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.Should().HaveCountGreaterThan(0);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().HaveCountGreaterThan(0);
            applicationWithRelatedData.Permissions.Should().HaveCountGreaterThan(0);
            applicationWithRelatedData.Roles.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = arrangeTestDataResponse.ActiveApplications[0].ApplicationId;

            var postReq = new FilterApplicationServiceRequest { IncludeRelated = false, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token, 
                RequestObject = postReq
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.Should().HaveCountGreaterThan(0);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().BeNull();
            applicationWithRelatedData.Permissions.Should().BeNull();
            applicationWithRelatedData.Roles.Should().BeNull();
            applicationWithRelatedData.RolePermissions.Should().BeNull();
            applicationWithRelatedData.ApplicationUserPermissions.Should().BeNull();
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationApiEndPoint, new FilterApplicationServiceRequest(), invalidToken);

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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultApplicationApiEndPoint, new FilterApplicationServiceRequest(), token);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = arrangeTestDataResponse.ActiveApplications[0].ApplicationId;
            
            var postReq = new FilterApplicationServiceRequest { ApplicationIds = new List<int> { applicationId }, IncludeRelated = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var application in result.Response)
            {
                _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(application, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = arrangeTestDataResponse.InactiveApplications[0].ApplicationId;
            
            var postReq = new FilterApplicationServiceRequest { ApplicationIds = new List<int> { applicationId }, IncludeRelated = true, IncludeInactive = true, DeleteCache = true};

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var application in result.Response)
            {
                _securityTestUtilities.Application.VerifyIncludeRelatedDataOnApplication(application, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterApplicationServiceRequest { IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            var activeRecordCt = result.Response.Count(x => x.Active);
            activeRecordCt.Should().Be(result.Response.Count);

            var readOnlyRecordCt = result.Response.Count(x => x.ReadOnly);
            readOnlyRecordCt.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Inactive_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterApplicationServiceRequest { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active && r.ReadOnly).ToList().Should().HaveCountGreaterThan(0); //activeReadOnlyRecords
            result.Response.Where(r => !r.Active && r.ReadOnly).ToList().Should().HaveCountGreaterThan(0); //inactiveReadOnlyRecords
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_ReadOnly_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications.First();

            var postReqInvalidName = new FilterApplicationServiceRequest { Name = testRecord.Name, DeleteCache = true };
            
            // Act
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultApplicationApiEndPoint, Token = token, RequestObject = postReqInvalidName });

            // Assert
            invalidNameResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var insertReq = _securityTestUtilities.Application.CreateInsertUpdateRequestWithRandomValues();

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = insertResult.Response.ApplicationId,
                Token = token
            });
            
            //Assert
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationApiEndPoint, "", token);
            
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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationApiEndPoint, new InsertUpdateApplicationRequest(), invalidToken);

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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultApplicationApiEndPoint, new InsertUpdateApplicationRequest(), token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var insertedRecord = arrangeTestDataResponse.ActiveApplications[0];

            var updateReq = new InsertUpdateApplicationRequest
            {
                Name = "Updated Application Name",
                Description = "Updated Application Description",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultApplicationApiEndPoint,
                RecordId = insertedRecord.ApplicationId,
                Token = token, 
                RequestObject = updateReq
            });

            //Assert
            updateResult.Response.ApplicationId.Should().Be(insertedRecord.ApplicationId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.CreatedOn.Should().NotBe(updateResult.Response.UpdatedOn);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationApiEndPoint, null, 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationApiEndPoint, "", 1, token);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationApiEndPoint, new InsertUpdateApplicationRequest(), 1, invalidToken);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationApiEndPoint, new InsertUpdateApplicationRequest(), 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications[0];

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedReadOnlyErrors();

            var updateReq = new InsertUpdateApplicationRequest
            {
                Name = "name update",
                Description = "description update",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultApplicationApiEndPoint, updateReq, testRecord.ApplicationId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(updateResult);

            // Assert
            errorValidationResult.Errors.Count.Should().Be(expectedFieldErrors.Count);
            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, errorValidationResult.Errors);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).First();

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationApiEndPoint,testRecord.ApplicationId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationApiEndPoint,testRecord.ApplicationId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultApplicationApiEndPoint,applicationId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationApiEndPoint,applicationId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.Application.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }

        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultApplicationApiEndPoint + "/" + applicationId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultApplicationApiEndPoint + "/" + applicationId);
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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationApiEndPoint, 1, invalidToken);

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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationApiEndPoint, 1, token);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications[0];

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedReadOnlyErrors();

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultApplicationApiEndPoint, testRecord.ApplicationId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            // Assert
            errorValidationResult.Errors.Count.Should().Be(expectedFieldErrors.Count);
            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, errorValidationResult.Errors);
        }

        #endregion

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { ApplicationReadOnly = true });
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
