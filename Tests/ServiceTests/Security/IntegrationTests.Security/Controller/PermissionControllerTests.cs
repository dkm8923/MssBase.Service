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
using Dto.Security.Application;
using IntegrationTests.Shared.Models;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class PermissionControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  IDefaultLogicTestsGetAllReadOnly,
                                                  IDefaultControllerTestsGetById,
                                                  IDefaultLogicTestsGetByIdReadOnly,
                                                  IDefaultControllerTestsFilter,
                                                  IDefaultLogicTestsFilterReadOnly,
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultLogicTestsUpdateReadOnly,
                                                  IDefaultControllerTestsDelete,
                                                  IDefaultLogicTestsDeleteReadOnly
    {
        private readonly HttpClient _client;
        private readonly string _defaultPermissionApiEndPoint = ApiEndPoints.Security.Permission.Base;

        public PermissionControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultPermissionApiEndPoint, invalidToken);

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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultPermissionApiEndPoint, token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<PermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            var readOnlyRecordCt = result.Response.Count(x => x.ReadOnly);
            readOnlyRecordCt.Should().Be(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactivePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactivePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";
           
            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultPermissionApiEndPoint + "/" + id);
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
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultPermissionApiEndPoint, 1, invalidToken);

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
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultPermissionApiEndPoint, 1, token);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeTrue();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactivePermissions.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeFalse();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = testRecord.PermissionId,
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterPermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterPermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var testPermission = arrangeTestDataResponse.ActivePermissions[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReqCreatedBy = new FilterPermissionServiceRequest { CreatedBy = TestConstants.CurrentUser };
            var postReqCreatedOnDate = new FilterPermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqUpdatedBy = new FilterPermissionServiceRequest { UpdatedBy = TestConstants.CurrentUser };
            var postReqUpdatedOnDate = new FilterPermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqName = new FilterPermissionServiceRequest { Name = testPermission.Name };
            
            // Act
            var filterCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqCreatedBy });
            var filterCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqCreatedOnDate });
            var filterUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqUpdatedBy });
            var filterUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqUpdatedOnDate });
            var filterNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqName });
            
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReqInvalidCreatedBy = new FilterPermissionServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterPermissionServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterPermissionServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterPermissionServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidName = new FilterPermissionServiceRequest { Name = "asdfasfasdfsd" };
            
            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint,Token = token, RequestObject = postReqInvalidName });
            
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultPermissionApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultPermissionApiEndPoint,"", token);
            
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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultPermissionApiEndPoint, new FilterPermissionServiceRequest(), invalidToken);

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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultPermissionApiEndPoint, new FilterPermissionServiceRequest(), token);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterPermissionServiceRequest { IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterPermissionServiceRequest { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultPermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions.First();

            var postReqInvalidName = new FilterPermissionServiceRequest { Name = testRecord.Name, DeleteCache = true };
            
            // Act
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<PermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultPermissionApiEndPoint, Token = token, RequestObject = postReqInvalidName });

            // Assert
            invalidNameResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var insertReq = _securityTestUtilities.Permission.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<PermissionDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<PermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = insertResult.Response.PermissionId,
                Token = token
            });
            
            //Assert
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultPermissionApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultPermissionApiEndPoint, "", token);
            
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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultPermissionApiEndPoint, new InsertUpdateApplicationRequest(), invalidToken);

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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultPermissionApiEndPoint, new InsertUpdateApplicationRequest(), token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
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
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<PermissionDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultPermissionApiEndPoint,
                RecordId = insertedRecord.PermissionId,
                Token = token, 
                RequestObject = updateReq
            });

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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultPermissionApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultPermissionApiEndPoint, "", 1, token);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultPermissionApiEndPoint, new InsertUpdateApplicationRequest(), 1, invalidToken);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultPermissionApiEndPoint, new InsertUpdateApplicationRequest(), 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions[0];

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedReadOnlyErrors();

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "name update",
                Description = "description update",
                Active = false,
                ApplicationId = testRecord.ApplicationId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultPermissionApiEndPoint, updateReq, testRecord.PermissionId, token);
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions[0];

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultPermissionApiEndPoint,testRecord.PermissionId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultPermissionApiEndPoint,testRecord.PermissionId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var permissionId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultPermissionApiEndPoint,permissionId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultPermissionApiEndPoint,permissionId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.Permission.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var permissionId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultPermissionApiEndPoint + "/" + permissionId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultPermissionApiEndPoint + "/" + permissionId);
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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultPermissionApiEndPoint, 1, invalidToken);

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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultPermissionApiEndPoint, 1, token);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyPermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActivePermissions[0];

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedReadOnlyErrors();

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultPermissionApiEndPoint, testRecord.PermissionId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            // Assert
            errorValidationResult.Errors.Count.Should().Be(expectedFieldErrors.Count);
            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, errorValidationResult.Errors);
        }

        #endregion

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { PermissionAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { PermissionReadOnly = true });
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
