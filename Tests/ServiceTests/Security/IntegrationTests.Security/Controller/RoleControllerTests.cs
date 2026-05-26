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
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().HaveCount(5);

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

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true) + "&" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);

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
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base + "?");

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base);

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
            var testRecord = arrangeTestDataResponse.ActiveRoles.First();
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var testRecord = arrangeTestDataResponse.InactiveRoles.First();

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();
            var testRecord = arrangeTestDataResponse.InactiveRoles.First();

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var testRecord = arrangeTestDataResponse.ActiveRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId, new BaseServiceGet { IncludeRelated = true });

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
            var testRecord = arrangeTestDataResponse.InactiveRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId, new BaseServiceGet { IncludeRelated = true, IncludeInactive = true });

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
            var testRecord = arrangeTestDataResponse.ActiveRoles.First();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId, new BaseServiceGet { IncludeRelated = false });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.RolePermissions.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + id);

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
            var response = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + id);
            var content = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Errors.Count.Should().Be(1);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestData();

            var postReq = new FilterRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

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

            var postReq = new FilterRoleServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

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
            var roles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            
            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { roles[0].RoleId, roles[1].RoleId } };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(2);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var postReq = new FilterRoleServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

            //Assert
            result.Response.Should().HaveCount(0);
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRoleTestDataWithRelatedData();
            var roles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            
            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { roles[0].RoleId, roles[1].RoleId }, IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

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
            var activeRoles = arrangeTestDataResponse.ActiveRoles.Take(5).ToList();
            var inactiveRoles = arrangeTestDataResponse.InactiveRoles.Take(5).ToList();

            var postReq = new FilterRoleServiceRequest { RoleIds = new List<int> { activeRoles[0].RoleId, activeRoles[1].RoleId, inactiveRoles[0].RoleId, inactiveRoles[1].RoleId }, IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

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
            var postReq = new FilterRoleServiceRequest();

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<RoleDto>>(_client, ApiEndPoints.Security.Role.Base, postReq);

            //Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            foreach (var role in result.Response)
            {
                role.RolePermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base + "/Filter", postReq);

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
            var insertReq = _securityTestUtilities.Role.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, insertReq);
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, insertResult.Response.RoleId);

            // Assert
            _securityTestUtilities.Role.VerifyTestRecordValuesMatch(insertResult.Response, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.Role.Base, postReq);

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
            var insertedRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);

            var updateReq = new InsertUpdateRoleRequest
            {
                Name = "name update",
                Description = "description update",
                Active = false,
                ApplicationId = application.ApplicationId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<RoleDto>(_client, ApiEndPoints.Security.Role.Base, updateReq, insertedRecord.RoleId);

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
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Role.Base + "/1", null);

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
            var response = await _client.PutAsync(ApiEndPoints.Security.Role.Base + "/1", postReq);

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
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId, false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Role.Base, testRecord.RoleId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + testRecord.RoleId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var roleId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + roleId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Role.Base + "/" + roleId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            var expectedInvalidDeleteError = _securityTestUtilities.Role.GetExpectedRecordDoesNotExistErrors();
            
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
            var roleId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Role.Base + "/" + roleId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Role.Base + "/" + roleId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
