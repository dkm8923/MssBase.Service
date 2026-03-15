using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserLogicTests : SecurityTestBase,
                                             IDefaultLogicTestsGetAll,
                                             IDefaultLogicTestsGetById,
                                             IDefaultLogicTestsFilter, 
                                             IDefaultLogicTestsInsert, 
                                             IDefaultLogicTestsUpdate,
                                             IDefaultLogicTestsDelete
    {
        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.DeleteAllRecords();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);

            var postReq = new FilterApplicationUserLogicRequest { };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId, false);

            var postReq = new FilterApplicationUserLogicRequest { IncludeInactive = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);

            var postReqInvalidEmail = new FilterApplicationUserServiceRequest { Email = "invalid@test.com" };
            var postReqInvalidApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = -1 };
            
            // Act
            var invalidEmailResult = await _applicationUserLogic.Filter(postReqInvalidEmail);
            var invalidApplicationIdResult = await _applicationUserLogic.Filter(postReqInvalidApplicationId);
            
            // Assert
            invalidEmailResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var applicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);

            //create test roles for filtering tests
            var testApplicationUser1 = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest
            {
                ApplicationId = applicationId,
                Email = "testEmail1@test.com",
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "TestPassword1!",
                Active = true,
                CurrentUser = "IntegrationTestInsert"
            }, _applicationLogic);

            var testApplicationUser2 = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest
            {
                ApplicationId = applicationId,
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 2, 2),
                Password = "TestPassword2!",
                Active = true,
                CurrentUser = "IntegrationTestInsert"
            }, _applicationLogic);

            await _applicationUserLogic.Update(testApplicationUser2.Response.ApplicationUserId, new InsertUpdateApplicationUserRequest
            {
                ApplicationId = applicationId,
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 2, 2),
                Password = "TestPassword2!",
                Active = true,
                CurrentUser = "IntegrationTestUpdate"
            }, _applicationLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationUserLogicRequest { CreatedBy = "IntegrationTestInsert" };
            var postReqFilterCreatedOnDate = new FilterApplicationUserLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationUserLogicRequest { UpdatedBy = "IntegrationTestUpdate" };
            var postReqFilterUpdatedOnDate = new FilterApplicationUserLogicRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationUserIds = new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { applicationUsers[0].ApplicationUserId, applicationUsers[1].ApplicationUserId, applicationUsers[2].ApplicationUserId } };
            var postReqFilterEmail = new FilterApplicationUserLogicRequest { Email = testApplicationUser1.Response.Email };
            var postReqFilterFirstName = new FilterApplicationUserLogicRequest { FirstName = testApplicationUser1.Response.FirstName };
            var postReqFilterLastName = new FilterApplicationUserLogicRequest { LastName = testApplicationUser1.Response.LastName };
            var postReqFilterDateOfBirth = new FilterApplicationUserLogicRequest { DateOfBirth = testApplicationUser1.Response.DateOfBirth };
            var postReqFilterApplicationId = new FilterApplicationUserLogicRequest { ApplicationId = applicationId };
            
            // Act
            var filterCreatedByResult = await _applicationUserLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _applicationUserLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _applicationUserLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationUserLogic.Filter(postReqFilterUpdatedOnDate);
            var filterApplicationUserIdsResult = await _applicationUserLogic.Filter(postReqFilterApplicationUserIds);
            var filterEmailResult = await _applicationUserLogic.Filter(postReqFilterEmail);
            var filterFirstNameResult = await _applicationUserLogic.Filter(postReqFilterFirstName);
            var filterLastNameResult = await _applicationUserLogic.Filter(postReqFilterLastName);
            var filterDateOfBirthResult = await _applicationUserLogic.Filter(postReqFilterDateOfBirth);
            var filterApplicationIdResult = await _applicationUserLogic.Filter(postReqFilterApplicationId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(2);
            filterCreatedOnDateResult.Response.Should().HaveCount(12);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(12);
            filterApplicationUserIdsResult.Response.Should().HaveCount(3);
            filterEmailResult.Response.Should().HaveCount(1);
            filterFirstNameResult.Response.Should().HaveCount(1);
            filterLastNameResult.Response.Should().HaveCount(1);
            filterDateOfBirthResult.Response.Should().HaveCount(1);
            filterApplicationIdResult.Response.Should().HaveCount(7);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(applicationId);

            // Act
            var result = await _applicationUserLogic.Insert(insertReq, _applicationLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.Email.Should().Be(insertReq.Email);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            var recordToCreate = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueEmailError = _securityTestUtilities.ApplicationUser.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueEmailError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = new InsertUpdateApplicationUserRequest();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task ApplicationUser_Insert_Should_Not_Create_Record_Invalid_Email_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(applicationId, true);
            recordToCreate.Email = "invalidEmail";

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedInvalidEmailFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Update_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = applicationId,
                CurrentUser = "IntegrationTest"
            };

            // Act
            var result = await _applicationUserLogic.Update(testRecord.ApplicationUserId, updateReq, _applicationLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Email.Should().Be(updateReq.Email);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecords = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            var recordToUpdate = testRecords.FirstOrDefault();
            var dupeEmail = testRecords.LastOrDefault().Email;

            var updateReq = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Email = dupeEmail;

            // Act
            var updateResult = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, updateReq, _applicationLogic);

            //Assert
            var expectedUniqueEmailError = _securityTestUtilities.ApplicationUser.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueEmailError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            var testRecords = await _applicationUserLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, new InsertUpdateApplicationUserRequest(), _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task ApplicationUser_Update_Should_Not_Create_Record_Invalid_Email_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            var testRecords = await _applicationUserLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();
            recordToUpdate.Email = "invalidEmail";

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedInvalidEmailFieldErrors();

            // Act
            var result = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(recordToUpdate), _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);

            // Act
            var result = await _applicationUserLogic.Delete(testRecord.ApplicationUserId);
            var getResult = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _applicationUserLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
