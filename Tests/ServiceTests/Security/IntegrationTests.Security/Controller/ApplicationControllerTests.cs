using Dto.Security.Application;
using Dto.Security.Application.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Models;
using System.Net;

namespace IntegrationTests.Security.Controller
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationControllerTests : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApplicationControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region utils
        #endregion

        //TODO: Clear all redis keys on each test run to ensure cache is not interfering with tests

        #region GetAll

        [Fact]
        public async Task Application_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords(1, false); //inactive record that should not be returned in results

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Application_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords(1, false); //inactive record that should be returned in results when includeInactive = true

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base + "?" + ControllerTestUtilities.createIncludeInactiveQueryStringParm(true));

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(6); //5 active + 1 inactive
        }

        [Fact]
        public async Task Application_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Application_GetById_Should_Return_Active_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);

            // Assert
            result.Errors.Should().HaveCount(0);
            _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Application_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Application_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId + "?" + ControllerTestUtilities.createIncludeInactiveQueryStringParm(true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Application_GetById_Should_Return_NotFound()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            var id = -1;

            // Act
            var response = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Application_GetById_Should_Return_Bad_Request_Invalid_Id()
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

        #endregion

        #region Filter

        [Fact]
        public async Task Application_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();

            var postReq = new FilterApplicationServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.ForEach(r => r.Active.Should().BeTrue());
        }

        [Fact]
        public async Task Application_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords(1, false); //inactive record that should be returned in results when includeInactive = true

            var postReq = new FilterApplicationServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(_client, ApiEndPoints.Security.Application.Base, postReq);

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0); //activeRecords
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0); //inactiveRecords
        }

        [Fact]
        public async Task Application_Filter_Should_Filter_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecordWithSpecificValues();

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "IntegrationTest" };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.Now) };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "IntegrationTest" };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.Now) };
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
        public async Task Application_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();

            var postReqInvalidCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidName = new FilterApplicationServiceRequest { Name = "TestApplicationName" };
            
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
        public async Task Application_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base + "/Filter", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Application_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            var postReq = ControllerTestUtilities.FormatPostRequest(null);

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base + "/Filter", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Application_Insert_Should_Create_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var insertedRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(_client, ApiEndPoints.Security.Application.Base, insertedRecord.ApplicationId);

            //Assert
            _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Application_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base, null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Application_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PostAsync(ApiEndPoints.Security.Application.Base, postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Application_Update_Should_Update_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var insertedRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdateApplicationRequest
            {
                Name = "Updated Application Name",
                Description = "Updated Application Description",
                Active = false,
                CurrentUser = "IntegrationTest"
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
        public async Task Application_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Application.Base + "/1", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Application_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var postReq = ControllerTestUtilities.FormatPostRequest(new object());

            // Act
            var response = await _client.PutAsync(ApiEndPoints.Security.Application.Base + "/1", postReq);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Application_Delete_Should_Delete_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Application_Delete_Should_Not_Delete_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var callerId = -1;

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + callerId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Application.Base + "/" + callerId);
            var errorValidationResult = await ControllerTestUtilities.GetResponseContent<ErrorValidationResult>(response);

            var expectedInvalidDeleteError = new Dictionary<string, List<string>>
            {
                { "Application", new List<string> { _validatorUtilities.CreateRecordDoesNotExistValidationErrorMessage("ApplicationId") } }
            };

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errorValidationResult.Errors.Should().BeEquivalentTo(expectedInvalidDeleteError);
        }

        [Fact]
        public async Task Application_Delete_Should_Return_Bad_Request_Invalid_Id()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var callerId = "asdfasfdasdfasfdas";

            // Act
            var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + callerId);
            var response = await _client.DeleteAsync(ApiEndPoints.Security.Application.Base + "/" + callerId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        #endregion

        //TODO: invalid values on insert / update. IE: overrunning db max lengths, required fields, etc
    }
}
