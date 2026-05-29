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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeInactive = true, DeleteCache = true } 
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10); //5 active + 5 inactive
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            
            // Act
            var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = true, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            foreach (var applicationWithRelatedData in result.Response)
            {
                applicationWithRelatedData.ApplicationUsers.Should().HaveCountGreaterThan(0);
                applicationWithRelatedData.Permissions.Should().HaveCountGreaterThan(0);
                applicationWithRelatedData.Roles.Should().HaveCountGreaterThan(0);
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);

            var applicationWithRelatedData = result.Response.Where(x => x.ApplicationId == arrangeTestDataResponse.ActiveApplications[0].ApplicationId).FirstOrDefault();
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.ActiveApplications[0];

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                RecordId = testRecord.ApplicationId,
                Token = token
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                RecordId = testRecord.ApplicationId,
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = arrangeTestDataResponse.InactiveApplications[0];
            
            // Act
            var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var id = "asfasdfasdfasdf";
            var apiEndpoint = ApiEndPoints.Security.Application.Base + "/" + id;

            // Act
            var result = await ControllerTestUtilities.GetRecordByIdBadRequestReturned<ApplicationDto>(_client,
                apiEndpoint,
                token
            );

            // Assert
            result.Should().BeTrue();
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                RecordId = id,
                Token = token,
                ExpectedStatusCode = HttpStatusCode.OK,
                QueryStringParms = new BaseServiceGet { IncludeRelated = false, DeleteCache = true }
            });

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
            var arrangeTestDataResponse = await ArrangeApplicationTestData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var postReq = new FilterApplicationServiceRequest { };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = ApiEndPoints.Security.Application.Base,
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
            var postReq = new FilterApplicationServiceRequest { IncludeInactive = true };

            // Act
            var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms
            {
                Client = _client,
                ApiEndPoint = ApiEndPoints.Security.Application.Base,
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
            await _securityTestUtilities.Application.CreateSingleApplicationTestRecordWithSpecificValues();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = TestConstants.CurrentUser };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqName = new FilterApplicationServiceRequest { Name = "Test Application Name" };   
            
            // Act
            var filterCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqCreatedBy });
            var filterCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqCreatedOnDate });
            var filterUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqUpdatedBy });
            var filterUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqUpdatedOnDate });
            var filterNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqName });
            
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

            var postReqInvalidCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidName = new FilterApplicationServiceRequest { Name = "asdfasfasdfsd" };
            
            // Act
            var invalidCreatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqInvalidCreatedBy });
            var invalidCreatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqInvalidCreatedOnDate });
            var invalidUpdatedByResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqInvalidUpdatedBy });
            var invalidUpdatedOnDateResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqInvalidUpdatedOnDate });
            var invalidNameResult = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(new HttpPostRequestParms { Client = _client, ApiEndPoint = ApiEndPoints.Security.Application.Base, Token = token, RequestObject = postReqInvalidName });
            
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
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, ApiEndPoints.Security.Application.Base, null, token);
            
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
            var result = await ControllerTestUtilities.GetFilteredRecords(_client, ApiEndPoints.Security.Application.Base, "", token);
            
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token, 
                RequestObject = postReq
            });

            // Assert
            result.Errors.Should().HaveCount(0);
            
            result.Response.Should().HaveCountGreaterThan(0);

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
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            var insertReq = _securityTestUtilities.Application.CreateInsertUpdateRequestWithRandomValues();

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecordWithValidationResult<ApplicationDto>(new HttpPostRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
                Token = token, 
                RequestObject = insertReq,
                ExpectedStatusCode = HttpStatusCode.Created
            });
            
            var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(new HttpGetRequestParms { 
                Client = _client, 
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, ApiEndPoints.Security.Application.Base, null, token);

            //Assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var insertResult = await ControllerTestUtilities.CreateRecord(_client, ApiEndPoints.Security.Application.Base, "", token);
            
            //assert
            insertResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var insertedRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

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
                ApiEndPoint = ApiEndPoints.Security.Application.Base, 
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
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, ApiEndPoints.Security.Application.Base, null, 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);

            // Act
            var updateResult = await ControllerTestUtilities.UpdateRecord(_client, ApiEndPoints.Security.Application.Base, "", 1, token);

            //Assert
            updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            // Act
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId, token);
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId, token);
            
            //Assert
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getByIdResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = -1;

            // Act
            var getByIdResult = await ControllerTestUtilities.GetRecordById(_client, ApiEndPoints.Security.Application.Base, applicationId, token);
            var deleteResult = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Application.Base, applicationId, token);
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
            var arrangeTestDataResponse = await ArrangeApplicationTestDataWithRelatedData();
            var token = await CreateAuthenticatedAdminTestUserAndReturnToken(arrangeTestDataResponse.ActiveApplications[0]);
            var applicationId = "asdfasfdasdfasfdas";

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, ApiEndPoints.Security.Application.Base + "/" + applicationId);
            ControllerTestUtilities.AddAuthorizationHeaderIfApplicable(getRequest, token);
            
            using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, ApiEndPoints.Security.Application.Base + "/" + applicationId);
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
