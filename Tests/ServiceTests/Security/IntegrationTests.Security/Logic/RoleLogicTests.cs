using Dto.Security.Role;
using Dto.Security.Role.Logic;
using Dto.Security.Role.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class RoleLogicTests : SecurityTestBase, 
                                        IDefaultLogicTestsGetAll,
                                        IDefaultLogicTestsGetById, 
                                        IDefaultLogicTestsInsert, 
                                        IDefaultLogicTestsUpdate,
                                        IDefaultLogicTestsDelete
    {
        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId, false);

            // Act
            var result = await _roleLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId, false);

            // Act
            var result = await _roleLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.Role.DeleteAllRecords();

            // Act
            var result = await _roleLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId);

            // Act
            var result = await _roleLogic.GetById(testRecord.RoleId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId, false);

            // Act
            var result = await _roleLogic.GetById(testRecord.RoleId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId, false);

            // Act
            var result = await _roleLogic.GetById(testRecord.RoleId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);

            var postReq = new FilterRoleLogicRequest { };

            // Act
            var result = await _roleLogic.Filter(postReq);

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
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);
            await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId, false);

            var postReq = new FilterRoleLogicRequest { IncludeInactive = true };

            // Act
            var result = await _roleLogic.Filter(postReq);

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
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);

            var postReqInvalidName = new FilterRoleLogicRequest { Name = "Invalid Name" };
            var postReqInvalidApplicationId = new FilterRoleLogicRequest { ApplicationId = -1 };
            
            // Act
            var invalidNameResult = await _roleLogic.Filter(postReqInvalidName);
            var invalidApplicationIdResult = await _roleLogic.Filter(postReqInvalidApplicationId);
            
            // Assert
            invalidNameResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertReq = _securityTestUtilities.Role.CreateInsertUpdateRequestWithRandomValues(applicationId);

            // Act
            var result = await _roleLogic.Insert(insertReq, _applicationLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.Name.Should().Be(insertReq.Name);
            result.Response.Description.Should().Be(insertReq.Description);
            result.Response.ApplicationId.Should().Be(insertReq.ApplicationId);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId);

            var recordToCreate = _securityTestUtilities.Role.ConvertRoleDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueNameError = _securityTestUtilities.Role.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _roleLogic.Insert(recordToCreate, _applicationLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueNameError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = new InsertUpdateRoleRequest();

            var expectedFieldErrors = _securityTestUtilities.Role.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _roleLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.Role.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.Role.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _roleLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Role_Insert_Should_Not_Create_Record_Invalid_ApplicationId_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.Role.CreateInsertUpdateRequestWithRandomValues(applicationId, true);
            recordToCreate.ApplicationId = -1;

            var expectedFieldErrors = _securityTestUtilities.Role.GetExpectedInvalidApplicationIdFieldErrors();

            // Act
            var result = await _roleLogic.Insert(recordToCreate, _applicationLogic);

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
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId);
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdateRoleRequest
            {
                Name = "Updated name",
                Description = "Updated description",
                Active = false,
                ApplicationId = newApplication.ApplicationId,
                CurrentUser = "IntegrationTestUpdate"
            };

            // Act
            var result = await _roleLogic.Update(testRecord.RoleId, updateReq, _applicationLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Name.Should().Be(updateReq.Name);
            result.Response.Description.Should().Be(updateReq.Description);
            result.Response.Active.Should().Be(updateReq.Active);
            result.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            result.Response.UpdatedBy.Should().Be(updateReq.CurrentUser);
            result.Response.CreatedOn.Should().NotBe(result.Response.UpdatedOn);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecords = await _securityTestUtilities.Role.CreateTestRecords(applicationId);
            var recordToUpdate = testRecords.FirstOrDefault();
            var dupeName = testRecords.LastOrDefault().Name;

            var updateReq = _securityTestUtilities.Role.ConvertRoleDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Name = dupeName;

            // Act
            var updateResult = await _roleLogic.Update(recordToUpdate.RoleId, updateReq, _applicationLogic);

            //Assert
            var expectedUniqueNameError = _securityTestUtilities.Role.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueNameError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Role.CreateTestRecords(applicationId);
            var testRecords = await _roleLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.Role.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _roleLogic.Update(recordToUpdate.RoleId, new InsertUpdateRoleRequest(), _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        //public async Task Role_Insert_Should_Not_Create_Record_Invalid_ApplicationId_Error()

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Role.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(applicationId);

            // Act
            var result = await _roleLogic.Delete(testRecord.RoleId);
            var getResult = await _roleLogic.GetById(testRecord.RoleId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.Role.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _roleLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
