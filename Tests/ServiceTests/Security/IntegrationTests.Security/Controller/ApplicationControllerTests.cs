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

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationControllerTests : SecurityTestBase, 
                                              IClassFixture<WebApplicationFactory<Program>>,
                                              IDefaultControllerTestsGetAll,
                                              IDefaultControllerTestsGetById,
                                              IDefaultControllerTestsFilter,
                                              IDefaultControllerTestsInsert,
                                              IDefaultControllerTestsUpdate,
                                              IDefaultControllerTestsDelete
    {
        private readonly HttpClient _client;

        //TODO: Include Related Testing
        //TODO: Clear all redis keys on each test run to ensure cache is not interfering with tests
        //TODO: DeleteCache Testing
        //TODO: When Logging is working, verify errors get logged after controller error

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
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateInactiveTestRecords(1); //inactive record that should not be returned in results

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateInactiveTestRecords(1); //inactive record that should be returned in results when includeInactive = true

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6); //5 active + 1 inactive
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applications = await _securityTestUtilities.Application.CreateActiveTestRecords();
            var applicationId = applications[0].ApplicationId;
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Application.CreateInactiveTestRecords(1); //inactive record that should be returned in results when includeInactive = true

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().HaveCount(5);
            applicationWithRelatedData.Permissions.Should().HaveCount(5);
            applicationWithRelatedData.Roles.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applications = await _securityTestUtilities.Application.CreateActiveTestRecords();
            var applicationId = applications[0].ApplicationId;
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Application.CreateInactiveTestRecords(1); //inactive record that should be returned in results when includeInactive = true

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base + "?" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(false));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().HaveCount(0);
            applicationWithRelatedData.Permissions.Should().HaveCount(0);
            applicationWithRelatedData.Roles.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId + "?" + ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_NotFound()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            var id = "asfasdfasdfasdf";

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + id);
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
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(testRecord.ApplicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(testRecord.ApplicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(testRecord.ApplicationId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base + "/" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(true), testRecord.ApplicationId);
            
            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.ApplicationUsers.Should().HaveCount(5);
            result.Response.Permissions.Should().HaveCount(5);
            result.Response.Roles.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(testRecord.ApplicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(testRecord.ApplicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(testRecord.ApplicationId);
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base + "/" + ControllerTestUtilities.CreateIncludeRelatedQueryStringParm(false), testRecord.ApplicationId);

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.ApplicationUsers.Should().HaveCount(0);
            result.Response.Permissions.Should().HaveCount(0);
            result.Response.Roles.Should().HaveCount(0);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();

            var postReq = new FilterApplicationServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);

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
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateInactiveTestRecords(1); //inactive record that should be returned in results when includeInactive = true

            var postReq = new FilterApplicationServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0); //activeRecords
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0); //inactiveRecords
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateSingleApplicationTestRecordWithSpecificValues();

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = TestConstants.CurrentUser };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqName = new FilterApplicationServiceRequest { Name = "Test Application Name" };   
            
            // Act
            var filterCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqCreatedBy);
            var filterCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqCreatedOnDate);
            var filterUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqUpdatedBy);
            var filterUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqUpdatedOnDate);
            var filterNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqName);
            
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
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();

            var postReqInvalidCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidName = new FilterApplicationServiceRequest { Name = "asdfasfasdfsd" };
            
            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqInvalidUpdatedOnDate);
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReqInvalidName);
            
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
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base + "/Filter", null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applications = await _securityTestUtilities.Application.CreateActiveTestRecords();
            var applicationId = applications[0].ApplicationId;
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(applicationId);
            
            var postReq = new FilterApplicationServiceRequest { IncludeRelated = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);
            
            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.Should().HaveCount(5);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().HaveCount(5);
            applicationWithRelatedData.Permissions.Should().HaveCount(5);
            applicationWithRelatedData.Roles.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applications = await _securityTestUtilities.Application.CreateActiveTestRecords();
            var applicationId = applications[0].ApplicationId;
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateActiveTestRecords(applicationId);
            
            var postReq = new FilterApplicationServiceRequest { IncludeRelated = false };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);
            
            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.Should().HaveCount(5);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == applicationId).FirstOrDefault();
            applicationWithRelatedData.ApplicationUsers.Should().HaveCount(0);
            applicationWithRelatedData.Permissions.Should().HaveCount(0);
            applicationWithRelatedData.Roles.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var insertedRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base, insertedRecord.ApplicationId);

            //Assert
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base, null);

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
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base, postReq);

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
            var insertedRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdateApplicationRequest
            {
                Name = "Updated Application Name",
                Description = "Updated Application Description",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base, updateReq, insertedRecord.ApplicationId);

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
            await ClearAllSecurityTestTableData();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Application.Base + "/1", null);

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
            var response = await _client.PutAsync(ApiEndPoints.Security.Application.Base + "/1", postReq);

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
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applicationId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + applicationId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Application.Base + "/" + applicationId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            //TODO: Use hardcoded string for testing. (Should be in application utilities)
            var expectedInvalidDeleteError = _securityTestUtilities.Application.GetExpectedRecordDoesNotExistErrors();
            
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
            var applicationId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + applicationId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Application.Base + "/" + applicationId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion
    }
}
