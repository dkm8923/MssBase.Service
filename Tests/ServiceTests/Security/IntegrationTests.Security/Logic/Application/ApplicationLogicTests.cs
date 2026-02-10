using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Security.Application.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Shared.Models;

namespace IntegrationTests.Security.Logic.Application
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationLogicTests : SecurityTestBase
    {
        #region GetAll

        [Fact]
        public async Task Application_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _SecurityLogic.Application.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Application_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _SecurityLogic.Application.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Application_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            // Act
            var result = await _SecurityLogic.Application.GetAll(new BaseLogicGet());

            // Assert
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
            var result = await _SecurityLogic.Application.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Application_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _SecurityLogic.Application.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Application_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _SecurityLogic.Application.GetById(testRecord.ApplicationId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Application_GetById_Unused_Id_Should_Return_Null()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            var id = -1;

            // Act
            var result = await _SecurityLogic.Application.GetById(id, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().BeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Application_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();

            var postReq = new FilterApplicationLogicRequest { };

            // Act
            var result = await _SecurityLogic.Application.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Application_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            var postReq = new FilterApplicationLogicRequest { IncludeInactive = true };

            // Act
            var result = await _SecurityLogic.Application.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0); //activeRecords
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0); //inactiveRecords
        }

        [Fact]
        public async Task Application_Filter_Should_Filter_Data()
        {
            //TODO: Test filtering by multiple application ids
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testData = await _SecurityTestUtilities.Application.CreateTestRecords();

            var testRecord = testData.FirstOrDefault();

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "IntegrationTest" };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.Now) };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "IntegrationTest" };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.Now) };
            var postReqName = new FilterApplicationServiceRequest { Name = testRecord.Name };
            
            // Act
            var filterCreatedByResult = await _SecurityLogic.Application.Filter(postReqCreatedBy);
            var filterCreatedOnDateResult = await _SecurityLogic.Application.Filter(postReqCreatedOnDate);
            var filterUpdatedByResult = await _SecurityLogic.Application.Filter(postReqUpdatedBy);
            var filterUpdatedOnDateResult = await _SecurityLogic.Application.Filter(postReqUpdatedOnDate);
            var filterNameResult = await _SecurityLogic.Application.Filter(postReqName);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCountGreaterThan(0);
            filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedByResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterNameResult.Response.Should().HaveCount(1);
            filterNameResult.Response.First().Name.Should().Be(testRecord.Name);

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
            var invalidCreatedByResult = await _SecurityLogic.Application.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _SecurityLogic.Application.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _SecurityLogic.Application.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _SecurityLogic.Application.Filter(postReqInvalidUpdatedOnDate);
            var invalidNameResult = await _SecurityLogic.Application.Filter(postReqInvalidName);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidNameResult.Response.Should().HaveCount(0);
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

            var insertCheck = await _SecurityLogic.Application.GetById(insertedRecord.ApplicationId, new BaseLogicGet());

            //Assert
            _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Application_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var recordToCreate = _SecurityTestUtilities.Application.ConvertApplicationDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueApplicationNameError = _SecurityTestUtilities.Application.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _SecurityLogic.Application.Insert(recordToCreate);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueApplicationNameError);
        }

        [Fact]
        public async Task Application_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var recordToCreate = new InsertUpdateApplicationRequest();

            var expectedFieldErrors = _SecurityTestUtilities.Application.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _SecurityLogic.Application.Insert(recordToCreate);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Application_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var recordToCreate = _SecurityTestUtilities.Application.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _SecurityTestUtilities.Application.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _SecurityLogic.Application.Insert(recordToCreate);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
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
            var updateResult = await _SecurityLogic.Application.Update(insertedRecord.ApplicationId, updateReq);

            //Assert
            updateResult.Response.ApplicationId.Should().Be(insertedRecord.ApplicationId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.CreatedOn.Should().NotBe(updateResult.Response.UpdatedOn);
        }

        [Fact]
        public async Task Application_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            var testRecords = await _SecurityTestUtilities.Application.CreateTestRecords();
            var recordToUpdate = testRecords.FirstOrDefault();
            var dupeApplicationName = testRecords.LastOrDefault().Name;

            var updateReq = _SecurityTestUtilities.Application.ConvertApplicationDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Name = dupeApplicationName;

            // Act
            var updateResult = await _SecurityLogic.Application.Update(recordToUpdate.ApplicationId, updateReq);

            //Assert
            var expectedUniqueUserFriendlyDescriptionError = _SecurityTestUtilities.Application.GetExpectedUniqueFieldErrors();

            //Assert
            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueUserFriendlyDescriptionError);
        }

        [Fact]
        public async Task Application_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            var testRecords = await _SecurityLogic.Application.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _SecurityTestUtilities.Application.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _SecurityLogic.Application.Update(recordToUpdate.ApplicationId, new InsertUpdateApplicationRequest());

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Application_Update_Should_Not_Update_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();
            await _SecurityTestUtilities.Application.CreateTestRecords();
            var testRecords = await _SecurityLogic.Application.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var updateReq = _SecurityTestUtilities.Application.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _SecurityTestUtilities.Application.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _SecurityLogic.Application.Update(recordToUpdate.ApplicationId, updateReq);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
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
            await _SecurityLogic.Application.Delete(testRecord.ApplicationId);
            var getResult = await _SecurityLogic.Application.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Application_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await _SecurityTestUtilities.Application.DeleteAllRecords();

            var expectedFieldErrors = _SecurityTestUtilities.Application.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _SecurityLogic.Application.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
