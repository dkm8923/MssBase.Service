using Dto.Security.Role;
using Dto.Security.Role.Service;
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
    public class RoleControllerTests : SecurityTestBase, 
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
        private readonly string _defaultRoleApiEndPoint = ApiEndPoints.Security.Role.Base;

        public RoleControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });
            
            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().HaveCountGreaterThan(0);

                foreach (var rolePermission in role.RolePermissions)
                {
                    rolePermission.Active.Should().BeTrue();
                    rolePermission.Permission.Should().NotBeNull();
                    rolePermission.Permission.Active.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Role.Base, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(10);

            foreach (var role in result.Response)
            {
                if (role.Active)
                {
                    role.RolePermissions.Should().HaveCountGreaterThan(0);
                }
                else
                {
                    role.RolePermissions.Should().HaveCountGreaterThan(0);
                }

                foreach (var rolePermission in role.RolePermissions)
                {
                    rolePermission.Permission.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Role.Base, 
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint, 
                Token = token
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRoles[0];

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                RecordId = testRecord.RoleId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRoles[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                RecordId = testRecord.RoleId,
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRoles[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                RecordId = testRecord.RoleId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var testRecord = arrangeTestDataResponse.ActiveRoles.First();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = ApiEndPoints.Security.Role.Base,
                RecordId = testRecord.RoleId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();

            result.Response.RolePermissions.Should().HaveCount(5);

            foreach (var rolePermission in result.Response.RolePermissions)
            {
                rolePermission.Active.Should().BeTrue();
                rolePermission.Permission.Should().NotBeNull();
                rolePermission.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = ApiEndPoints.Security.Role.Base,
                RecordId = testRecord.RoleId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
             });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
            result.Response.RolePermissions.Should().HaveCount(5);

            foreach (var rolePermission in result.Response.RolePermissions)
            {
                rolePermission.Active.Should().BeFalse();
                rolePermission.Permission.Should().NotBeNull();
                rolePermission.Permission.Active.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = ApiEndPoints.Security.Role.Base,
                RecordId = testRecord.RoleId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false }
            });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.RolePermissions.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";

            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultRoleApiEndPoint + "/" + id);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getByIdRequest, token);
            
            // Act
            var getResponse = await _client.SendAsync(getByIdRequest);
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReq = new FilterRoleServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var roles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            
            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { roles[0].RoleId, roles[1].RoleId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Response.Should().HaveCount(2);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            var postReqInvalidCreatedBy = new FilterRoleServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterRoleServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterRoleServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterRoleServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidRoleIds = new FilterRoleServiceRequest { RoleIds = new List<int> { 9999 } };
            var postReqInvalidName = new FilterRoleServiceRequest { Name = "InvalidName" };
            var postReqInvalidApplicationId = new FilterRoleServiceRequest { ApplicationId = 9999 };

            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidRoleIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidRoleIds });
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidName });
            var invalidApplicationIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultRoleApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationId });

            //Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidRoleIdsResult.Response.Should().HaveCount(0);
            invalidNameResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var roles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            
            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { roles[0].RoleId, roles[1].RoleId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);

            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
                r.RolePermissions.Should().HaveCount(5);

                foreach (var rolePermission in r.RolePermissions)
                {
                    rolePermission.Active.Should().BeTrue();
                    rolePermission.Permission.Should().NotBeNull();
                    rolePermission.Permission.Active.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var activeRoles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            var inactiveRoles = arrangeTestDataResponse.InactiveRoles.Take(5).ToList();

            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { activeRoles[0].RoleId, activeRoles[1].RoleId, inactiveRoles[0].RoleId, inactiveRoles[1].RoleId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(4);

            foreach (var role in result.Response)
            {
                if (role.Active)
                {
                    role.RolePermissions.Should().HaveCount(10);
                }
                else
                {
                    role.RolePermissions.Should().HaveCount(5);
                }

                foreach (var rolePermission in role.RolePermissions)
                {
                    rolePermission.Permission.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterRoleServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultRoleApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(5);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRoleApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultRoleApiEndPoint,"", token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var application = arrangeTestDataResponse.ActiveApplications[0];
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            var insertReq = _securityTestUtilities.Role.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<RoleDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                RecordId = insertResult.Response.RoleId,
                Token = token
            });

            // Assert
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRoleApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
             // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultRoleApiEndPoint, "", token);
            
            //assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var insertedRecord = arrangeTestDataResponse.ActiveRoles.FirstOrDefault();

            var updateReq = new InsertUpdateRoleRequest
            {
                Name = "name update",
                Description = "description update",
                Active = false,
                ApplicationId = insertedRecord.ApplicationId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<RoleDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultRoleApiEndPoint,
                RecordId = insertedRecord.RoleId,
                Token = token, 
                RequestObject = updateReq
            });

            // Assert
            updateResult.Response.RoleId.Should().Be(insertedRecord.RoleId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRoleApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultRoleApiEndPoint, "", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveRoles[0];

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRoleApiEndPoint,testRecord.RoleId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRoleApiEndPoint,testRecord.RoleId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var roleId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultRoleApiEndPoint,roleId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultRoleApiEndPoint,roleId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.Role.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var roleId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultRoleApiEndPoint + "/" + roleId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultRoleApiEndPoint + "/" + roleId);
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
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { RoleAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken(ApplicationDto application)
        {
            return await CreateAuthenticatedTestUserAndReturnToken(application, new AssignRoleRequest { RoleReadOnly = true });
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
