using Dto.Security.User;
using Dto.Security.User.Service;
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
using Shared.Models.Dtos;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class UserControllerTests : SecurityTestBase, 
                                                  IClassFixture<WebApplicationFactory<Program>>,
                                                  IDefaultControllerTestsGetAll,
                                                  //IDefaultControllerTestsGetAllIncludeRelated,
                                                  IDefaultLogicTestsGetAllReadOnly,
                                                  IDefaultControllerTestsGetById,
                                                  //IDefaultControllerTestsGetByIdIncludeRelated,
                                                  IDefaultLogicTestsGetByIdReadOnly,
                                                  IDefaultControllerTestsGetAuditLogsById,
                                                  //IDefaultControllerTestsFilter,
                                                  //IDefaultControllerTestsFilterIncludeRelated,
                                                  IDefaultLogicTestsFilterReadOnly,  
                                                  IDefaultControllerTestsInsert,
                                                  IDefaultControllerTestsUpdate,
                                                  IDefaultControllerTestsDelete,
                                                  IDefaultLogicTestsDeleteReadOnly
    {
        private readonly HttpClient _client;
        private readonly string _defaultUserApiEndPoint = ApiEndPoints.Security.User.Base;

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        // [Fact]
        // public async Task Default_GetAll_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            
        //     // Act
        //     var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
        //     });

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCountGreaterThan(0);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
        //     }
        // }

        // [Fact]
        // public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            
        //     // Act
        //     var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
        //     });

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(3);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
        //     }
        // }

        // [Fact]
        // public async Task Default_GetAll_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            
        //     // Act
        //     var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { DeleteCache = true } 
        //     });

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(2);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         applicationUser.ApplicationUserPermissions.Should().BeNull();
        //         applicationUser.ApplicationUserRoles.Should().BeNull();
        //     }
        // }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint, 
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
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint, invalidToken);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());
            
            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint, token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetAllByUserId_Should_Return_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];
            var pswdChangeHistoryResponse = await ArrangeUserPasswordChangeHistoryTestData(testRecord.UserId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<IEnumerable<UserLogChangePasswordDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint + "/PasswordChangeHistory",
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);
            
            foreach (var record in result.Response)
            {
                record.UserId.Should().Be(testRecord.UserId);
                record.OldPassword.Should().NotBeNullOrEmpty();
                record.CreatedBy.Should().NotBeNullOrEmpty();
                record.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(5));
            }
        }

        [Fact]
        public async Task PasswordChangeHistory_GetAllByUserId_Should_Not_Return_Record_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];
            var pswdChangeHistoryResponse = await ArrangeUserPasswordChangeHistoryTestData(testRecord.UserId);
            var invalidUserId = "asdfasfdasdfasfdas";
            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultUserApiEndPoint + "/PasswordChangeHistory/" + invalidUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);

            // Act
            var getResponse = await _client.SendAsync(getRequest);
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetAllByUserId_Should_Not_Return_Record_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint + "/PasswordChangeHistory/1", invalidToken);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetAllByUserId_Should_Not_Return_Record_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());
            
            // Act
            var getAllResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint + "/PasswordChangeHistory/1", token);

            //Assert
            getAllResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetAllByUserId_Should_Not_Return_Record_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];
            var pswdChangeHistoryResponse = await ArrangeUserPasswordChangeHistoryTestData(testRecord.UserId);
            var invalidUserId = -1;
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<IEnumerable<UserLogChangePasswordDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint + "/PasswordChangeHistory",
                RecordId = invalidUserId,
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            await _securityTestUtilities.User.CreateSingleUserTestRecord();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<UserDto>>(new HttpGetRequestParms {
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint, 
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.User.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.InactiveUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true },
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.InactiveUsers[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().NotBeNull();
            _securityTestUtilities.User.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        // [Fact]
        // public async Task Default_GetById_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var testRecord = arrangeTestDataResponse.ActiveUsers.First();

        //     // Act
        //     var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         RecordId = testRecord.UserId,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
        //     });

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.Active.Should().BeTrue();

        //     _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: false);
        // }

        // [Fact]
        // public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var testRecord = arrangeTestDataResponse.InactiveUsers.First();

        //     // Act
        //     var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         RecordId = testRecord.UserId,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { IncludeRelated = true, IncludeInactive = true, DeleteCache = true }
        //     });

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.Active.Should().BeFalse();
        //     _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: true);
        // }

        // [Fact]
        // public async Task Default_GetById_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var testRecord = arrangeTestDataResponse.ActiveUsers.First();

        //     // Act
        //     var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         RecordId = testRecord.UserId,
        //         Token = token,
        //         QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
        //     });

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.ApplicationUserPermissions.Should().BeNull();
        //     result.Response.ApplicationUserRoles.Should().BeNull();
        // }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var id = "asfasdfasdfasdf";
           
            using var getByIdRequest = new HttpRequestMessage(HttpMethod.Get, _defaultUserApiEndPoint + "/" + id);
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
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultUserApiEndPoint, 1, invalidToken);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultUserApiEndPoint, 1, token);

            //Assert
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.User.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeTrue();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.InactiveUsers.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.User.VerifyTestRecordValuesMatch(result.Response, testRecord);
            result.Response.Active.Should().BeFalse();
            result.Response.ReadOnly.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers.Where(x => x.ReadOnly).FirstOrDefault();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true },
                ExpectedStatusCode = System.Net.HttpStatusCode.NotFound
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().BeNull();
        }

        #endregion

        #region GetAuditLogById

        [Fact]
        public async Task Default_GetAuditLogById_Should_Return_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];

            var updateReq = new InsertUpdateUserRequest
            {
                FirstName = "Updated First Name",
                LastName = "Updated Last Name",
                Email = "updatedemail@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<UserDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token, 
                RequestObject = updateReq
            });
            
            // Act
            var result = await ControllerTestUtilities.GetAuditLogRecordsByIdWithValidationResult<AuditLogDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = testRecord.UserId,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAuditLogById_Should_Return_Unauthorized()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidToken = "someInvalidToken";

            // Act
            var getAuditLogByIdResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint + "/1/AuditLogs", invalidToken);

            //Assert
            getAuditLogByIdResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_GetAuditLogById_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());
            
            // Act
            var getAuditLogByIdResult = await ControllerTestUtilities.GetAllRecords(_client, _defaultUserApiEndPoint + "/1/AuditLogs", token);

            //Assert
            getAuditLogByIdResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_GetAuditLogById_Should_Return_NotFound()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var id = -1;

            // Act
            var result = await ControllerTestUtilities.GetAuditLogRecordsByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = id,
                Token = token,
                QueryStringParms = new BaseServiceGet { DeleteCache = true },
                ExpectedStatusCode = HttpStatusCode.NotFound
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var postReq = new FilterUserServiceRequest { DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var postReq = new FilterUserServiceRequest { IncludeInactive = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            
            var postReq = new FilterUserServiceRequest { UserIds = new List<int>(), DeleteCache = true };
            arrangeTestDataResponse.ActiveUsers.ForEach(au => postReq.UserIds.Add(au.UserId));

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
                Token = token,
                RequestObject = postReq
            });

            //Assert
            result.Response.Should().HaveSameCount(arrangeTestDataResponse.ActiveUsers);
        }

        // [Fact]
        // public async Task Default_Filter_Should_Return_Zero_Records()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            
        //     var postReqInvalidCreatedBy = new FilterUserServiceRequest { CreatedBy = "TestCreatedBy", DeleteCache = true };
        //     var postReqInvalidCreatedOnDate = new FilterUserServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000"), DeleteCache = true };
        //     var postReqInvalidUpdatedBy = new FilterUserServiceRequest { UpdatedBy = "TestUpdatedBy", DeleteCache = true };
        //     var postReqInvalidUpdatedOnDate = new FilterUserServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000"), DeleteCache = true };
        //     var postReqInvalidUserIds = new FilterUserServiceRequest { UserIds = new List<int> { 9999 }, DeleteCache = true };
        //     var postReqInvalidEmail = new FilterUserServiceRequest { Email = "invalidemail@test.com", DeleteCache = true };
        //     var postReqInvalidFirstName = new FilterUserServiceRequest { FirstName = "InvalidFirstName", DeleteCache = true };
        //     var postReqInvalidLastName = new FilterUserServiceRequest { LastName = "InvalidLastName", DeleteCache = true };
        //     var postReqInvalidDateOfBirth = new FilterUserServiceRequest { DateOfBirth = LogicTestUtilities.GetRandomDateTime(1999), DeleteCache = true };
        //     var postReqInvalidApplicationId = new FilterUserServiceRequest { ApplicationId = 9999, DeleteCache = true };

        //     // Act
        //     var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedBy });
        //     var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidCreatedOnDate });
        //     var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedBy });
        //     var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidUpdatedOnDate });
        //     var invalidUserIdsResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidUserIds });
        //     var invalidEmailResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidEmail });
        //     var invalidFirstNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidFirstName });
        //     var invalidLastNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidLastName });
        //     var invalidDateOfBirthResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidDateOfBirth });
        //     var invalidApplicationIdResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint,Token = token, RequestObject = postReqInvalidApplicationId });

        //     //Assert
        //     invalidCreatedByResult.Response.Should().HaveCount(0);
        //     invalidCreatedOnDateResult.Response.Should().HaveCount(0);
        //     invalidUpdatedByResult.Response.Should().HaveCount(0);
        //     invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
        //     invalidUserIdsResult.Response.Should().HaveCount(0);
        //     invalidEmailResult.Response.Should().HaveCount(0);
        //     invalidFirstNameResult.Response.Should().HaveCount(0);
        //     invalidLastNameResult.Response.Should().HaveCount(0);
        //     invalidDateOfBirthResult.Response.Should().HaveCount(0);
        //     invalidApplicationIdResult.Response.Should().HaveCount(0);
        // }
        
        // [Fact]
        // public async Task Default_Filter_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var applicationUserId = arrangeTestDataResponse.ActiveUsers[0].ApplicationUserId;
            
        //     var postReq = new FilterUserServiceRequest { UserIds = new List<int> { applicationUserId }, IncludeRelated = true, DeleteCache = true };

        //     // Act
        //     var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
        //     {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         RequestObject = postReq
        //     });

        //     //Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(1);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
        //     }
        // }

        // [Fact]
        // public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var applicationUserId = arrangeTestDataResponse.InactiveUsers[0].ApplicationUserId;
            
        //     var postReq = new FilterUserServiceRequest { UserIds = new List<int> { applicationUserId }, IncludeRelated = true, IncludeInactive = true, DeleteCache = true };

        //     // Act
        //     var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
        //     {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         RequestObject = postReq
        //     });

        //     //Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(1);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.User.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
        //     }
        // }

        // [Fact]
        // public async Task Default_Filter_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
        //     var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
        //     var applicationUserId = arrangeTestDataResponse.ActiveUsers[0].ApplicationUserId;
            
        //     var postReq = new FilterUserServiceRequest { UserIds = new List<int> { applicationUserId }, DeleteCache = true };

        //     // Act
        //     var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
        //     {
        //         Client = _client,
        //         ApiEndPoint = _defaultUserApiEndPoint,
        //         Token = token,
        //         RequestObject = postReq
        //     });

        //     //Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(1);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         applicationUser.ApplicationUserPermissions.Should().BeNull();
        //         applicationUser.ApplicationUserRoles.Should().BeNull();
        //     }
        // }

        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultUserApiEndPoint, null, token);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultUserApiEndPoint,"", token);
            
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
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultUserApiEndPoint, new FilterUserServiceRequest(), invalidToken);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());

            // Act
            var filterResult = await ControllerTestUtilities.GetFilteredRecords(_client, _defaultUserApiEndPoint, new FilterUserServiceRequest(), token);

            //Assert
            filterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            var postReq = new FilterUserServiceRequest { IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            var postReq = new FilterUserServiceRequest { IncludeInactive = true, IncludeReadOnly = true, DeleteCache = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = _defaultUserApiEndPoint,
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
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers.First();

            var postReqInvalidFirstName = new FilterUserServiceRequest { FirstName = testRecord.FirstName, DeleteCache = true };
            
            // Act
            var invalidFirstNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<UserDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = _defaultUserApiEndPoint, Token = token, RequestObject = postReqInvalidFirstName });

            // Assert
            invalidFirstNameResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            var insertReq = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<UserDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<UserDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = insertResult.Response.UserId,
                Token = token
            });
            
            //Assert
            _securityTestUtilities.User.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultUserApiEndPoint, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultUserApiEndPoint, "", token);
            
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
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultUserApiEndPoint, new InsertUpdateUserRequest(), invalidToken);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, _defaultUserApiEndPoint, new InsertUpdateUserRequest(), token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var insertedRecord = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();

            var updateReq = new InsertUpdateUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<UserDto>(new HttpPutRequestParms { 
                Client = _client, 
                ApiEndPoint = _defaultUserApiEndPoint,
                RecordId = insertedRecord.UserId,
                Token = token, 
                RequestObject = updateReq
            });
           
            // Assert
            updateResult.Response.UserId.Should().Be(insertedRecord.UserId);
            updateResult.Response.Email.Should().Be(updateReq.Email);
            updateResult.Response.FirstName.Should().Be(updateReq.FirstName);
            updateResult.Response.LastName.Should().Be(updateReq.LastName);
            updateResult.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultUserApiEndPoint,"", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultUserApiEndPoint, "", 1, token);

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
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultUserApiEndPoint, new InsertUpdateUserRequest(), 1, invalidToken);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultUserApiEndPoint, new InsertUpdateUserRequest(), 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedReadOnlyErrors();

            var updateReq = new InsertUpdateUserRequest
            {
                FirstName = "first name update",
                LastName = "last name update",
                Email = "emailupdate@test.com",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, _defaultUserApiEndPoint, updateReq, testRecord.UserId, token);
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];
            
            //delete the application user after successful auth so that the user can be deleted
            await _applicationUserLogic.Delete(arrangeTestDataResponse.ActiveApplicationUsers[0].ApplicationUserId, TestConstants.CurrentUser); 
            
            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultUserApiEndPoint,testRecord.UserId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultUserApiEndPoint,testRecord.UserId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var applicationUserId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, _defaultUserApiEndPoint, applicationUserId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultUserApiEndPoint, applicationUserId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            var expectedInvalidDeleteError = _securityTestUtilities.User.GetExpectedRecordDoesNotExistErrors();
            
            // Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }
        
        [Fact]
        public async Task Default_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var applicationUserId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, _defaultUserApiEndPoint + "/" + applicationUserId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, _defaultUserApiEndPoint + "/" + applicationUserId);
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
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultUserApiEndPoint, 1, invalidToken);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Default_Delete_Should_Return_Forbidden()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest());

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultUserApiEndPoint, 1, token);

            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken();
            var testRecord = arrangeTestDataResponse.ActiveUsers[0];

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedReadOnlyErrors();

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, _defaultUserApiEndPoint, testRecord.UserId, token);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(deleteResult);

            // Assert
            errorValidationResult.Errors.Count.Should().Be(expectedFieldErrors.Count);
            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, errorValidationResult.Errors);
        }

        #endregion

        #region Private

        private async Task<string> CreateAuthenticatedAdminTestUserAndReturnToken()
        {
            return await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest { UserAdmin = true });
        }

        //TODO: Create Readonly User Tests
        private async Task<string> CreateAuthenticatedReadOnlyTestUserAndReturnToken()
        {
            return await CreateAuthenticatedTestUserAndReturnToken(new AssignRoleRequest { UserReadOnly = true });
        }

        private async Task<string> CreateAuthenticatedTestUserAndReturnToken(AssignRoleRequest assignRoleRequest)
        {
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            //authenticate test user and receive token to be used in authorized controller calls
            var testUser = await CreateTestUserWithPermissions(application.ApplicationId, assignRoleRequest);
            var authResult = await ControllerTestUtilities.AuthenticateTestUserAndReturnAuthToken(_client, testUser.Email, TestConstants.DefaultTestUserPassword, application.Name);
            
            return authResult.Token;
        }

        #endregion
    }
}
