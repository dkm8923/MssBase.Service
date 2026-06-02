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
using Dto.Security.Application;
using IntegrationTests.Shared.Models;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class RolePermissionControllerTests : SecurityTestBase, 
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
        private readonly string _defaultRolePermissionApiEndPoint = ApiEndPoints.Security.RolePermission.Base;

        public RolePermissionControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Should().NotBeNull();
                rolePermission.Active.Should().BeTrue();
                rolePermission.Permission.Should().NotBeNull();
                rolePermission.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(15);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RolePermissionDto>>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultRolePermissionApiEndPoint, invalidToken);

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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultRolePermissionApiEndPoint, token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
             // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";
           
            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultRolePermissionApiEndPoint + "/" + id);
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = testRecord.RolePermissionId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRolePermissionApiEndPoint, 1, invalidToken);

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
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRolePermissionApiEndPoint, 1, token);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterRolePermissionServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterRolePermissionServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var rolePermissionIds = new List<int> 
            { 
                arrangeTestDataResponse.ActiveRolePermissions[0].RolePermissionId, 
                arrangeTestDataResponse.ActiveRolePermissions[1].RolePermissionId,
                arrangeTestDataResponse.ActiveRolePermissions[2].RolePermissionId 
            };
            
            var postReq = new FilterRolePermissionServiceRequest { RolePermissionIds = rolePermissionIds };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            var postReq = new FilterRolePermissionServiceRequest { };
            var postReqInvalidCreatedBy = new FilterRolePermissionServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterRolePermissionServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterRolePermissionServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterRolePermissionServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { 9999 } };
            var postReqInvalidApplicationId = new FilterRolePermissionServiceRequest { ApplicationId = 9999 };
            var postReqInvalidRoleId = new FilterRolePermissionServiceRequest { RoleId = 9999 };
            var postReqInvalidPermissionId = new FilterRolePermissionServiceRequest { PermissionId = 9999 };

            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidRolePermissionIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidRolePermissionIds });
            var invalidApplicationIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationId });
            var invalidRoleIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidRoleId });
            var invalidPermissionIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRolePermissionApiEndPoint,Token = token, RequestObject = postReqInvalidPermissionId });
            
            //Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidRolePermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidRoleIdResult.Response.Should().HaveCount(0);
            invalidPermissionIdResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var rolePermissions = arrangeTestDataResponse.ActiveRolePermissions.Take(5).ToList();
            
            var postReq = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { rolePermissions[0].RolePermissionId, rolePermissions[1].RolePermissionId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var activeRolePermissions = arrangeTestDataResponse.ActiveRolePermissions.Take(5).ToList();
            var inactiveRolePermissions = arrangeTestDataResponse.InactiveRolePermissions.Take(5).ToList();

            var postReq = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { activeRolePermissions[0].RolePermissionId, activeRolePermissions[1].RolePermissionId, inactiveRolePermissions[0].RolePermissionId, inactiveRolePermissions[1].RolePermissionId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(4);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterRolePermissionServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RolePermissionDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().BeNull();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRolePermissionApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRolePermissionApiEndPoint,"", token);
            
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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRolePermissionApiEndPoint, new FilterRolePermissionServiceRequest(), invalidToken);

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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRolePermissionApiEndPoint, new FilterRolePermissionServiceRequest(), token);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(application);
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
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<RolePermissionDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RolePermissionDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = insertResult.Response.RolePermissionId,
                Token = token
            });

            // Assert
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRolePermissionApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRolePermissionApiEndPoint, "", token);
            
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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRolePermissionApiEndPoint, new InsertUpdateApplicationRequest(), invalidToken);

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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRolePermissionApiEndPoint, new InsertUpdateApplicationRequest(), token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(application);
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

            var insertedRecordRes = await ControllerTestUtilities.CreateRecordWithValidationResult<RolePermissionDto>(new HttpPostRequestParms {
                Client = _client,
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                Token = token,
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });

            var updateReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = inactivePermission.PermissionId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<RolePermissionDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRolePermissionApiEndPoint,
                RecordId = insertedRecordRes.Response.RolePermissionId,
                Token = token, 
                RequestObject = updateReq
            });

            // Assert
            updateResult.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            updateResult.Response.RoleId.Should().Be(updateReq.RoleId);
            updateResult.Response.PermissionId.Should().Be(updateReq.PermissionId);
            updateResult.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRolePermissionApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRolePermissionApiEndPoint, "", 1, token);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRolePermissionApiEndPoint, new InsertUpdateApplicationRequest(), 1, invalidToken);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRolePermissionApiEndPoint, new InsertUpdateApplicationRequest(), 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions[0];

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRolePermissionApiEndPoint, testRecord.RolePermissionId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRolePermissionApiEndPoint, testRecord.RolePermissionId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRolePermissionApiEndPoint, applicationUserId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRolePermissionApiEndPoint, applicationUserId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.RolePermission.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationUserId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultRolePermissionApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultRolePermissionApiEndPoint + "/" + applicationUserId);
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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRolePermissionApiEndPoint, 1, invalidToken);

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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRolePermissionApiEndPoint, 1, token);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { RolePermissionAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { RolePermissionReadOnly = true });
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
