using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using Dto.Security.Permission.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class PermissionLogicTests : SecurityTestBase, 
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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId, false);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId, false);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.Permission.DeleteAllRecords();

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId);

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId, false);

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId, false);

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);

            var postReq = new FilterPermissionLogicRequest { };

            // Act
            var result = await _permissionLogic.Filter(postReq);

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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId, false);

            var postReq = new FilterPermissionLogicRequest { IncludeInactive = true };

            // Act
            var result = await _permissionLogic.Filter(postReq);

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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);

            var postReqInvalidName = new FilterPermissionLogicRequest { Name = "Invalid Name" };
            var postReqInvalidApplicationId = new FilterPermissionLogicRequest { ApplicationId = -1 };
            
            // Act
            var invalidNameResult = await _permissionLogic.Filter(postReqInvalidName);
            var invalidApplicationIdResult = await _permissionLogic.Filter(postReqInvalidApplicationId);
            
            // Assert
            invalidNameResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var permissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);

            //create test permissions for filtering tests
            var testPermission1 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 1",
                Description = "Test Permission Description 1",
                Active = true,
                CurrentUser = "IntegrationTestInsert"
            }, _applicationLogic);

            var testPermission2 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 2",
                Description = "Test Permission Description 2",
                Active = true,
                CurrentUser = "IntegrationTestInsert"
            }, _applicationLogic);

            await _permissionLogic.Update(testPermission2.Response.PermissionId, new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 2",
                Description = "Test Permission Description 2",
                Active = true,
                CurrentUser = "IntegrationTestUpdate"
            }, _applicationLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterPermissionLogicRequest { CreatedBy = "IntegrationTestInsert" };
            var postReqFilterCreatedOnDate = new FilterPermissionLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterPermissionLogicRequest { UpdatedBy = "IntegrationTestUpdate" };
            var postReqFilterUpdatedOnDate = new FilterPermissionLogicRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterPermissionIds = new FilterPermissionLogicRequest { PermissionIds = new List<int> { permissions[0].PermissionId, permissions[1].PermissionId, permissions[2].PermissionId } };
            var postReqFilterName = new FilterPermissionLogicRequest { Name = "Test Permission Name 1" };
            var postReqFilterApplicationId = new FilterPermissionLogicRequest { ApplicationId = applicationId };
            
            // Act
            var filterCreatedByResult = await _permissionLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _permissionLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _permissionLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _permissionLogic.Filter(postReqFilterUpdatedOnDate);
            var filterPermissionIdsResult = await _permissionLogic.Filter(postReqFilterPermissionIds);
            var filterNameResult = await _permissionLogic.Filter(postReqFilterName);
            var filterApplicationIdResult = await _permissionLogic.Filter(postReqFilterApplicationId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(2);
            filterCreatedOnDateResult.Response.Should().HaveCount(7);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(7);
            filterPermissionIdsResult.Response.Should().HaveCount(3);
            filterNameResult.Response.Should().HaveCount(1);
            filterApplicationIdResult.Response.Should().HaveCount(7);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var insertReq = _securityTestUtilities.Permission.CreateInsertUpdateRequestWithRandomValues(applicationId);

            // Act
            var result = await _permissionLogic.Insert(insertReq, _applicationLogic);

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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId);

            var recordToCreate = _securityTestUtilities.Permission.ConvertPermissionDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueNameError = _securityTestUtilities.Permission.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _permissionLogic.Insert(recordToCreate, _applicationLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueNameError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = new InsertUpdatePermissionRequest();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _permissionLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.Permission.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _permissionLogic.Insert(recordToCreate, _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Permission_Insert_Should_Not_Create_Record_Invalid_ApplicationId_Error()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var recordToCreate = _securityTestUtilities.Permission.CreateInsertUpdateRequestWithRandomValues(applicationId, true);
            recordToCreate.ApplicationId = applicationId > 1 ? applicationId - 1 : applicationId + 1;

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedInvalidApplicationIdFieldErrors();

            // Act
            var result = await _permissionLogic.Insert(recordToCreate, _applicationLogic);

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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId);
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "Updated name",
                Description = "Updated description",
                Active = false,
                ApplicationId = newApplication.ApplicationId,
                CurrentUser = "IntegrationTestUpdate"
            };

            // Act
            var result = await _permissionLogic.Update(testRecord.PermissionId, updateReq, _applicationLogic);

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
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecords = await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            var recordToUpdate = testRecords.FirstOrDefault();
            var dupeName = testRecords.LastOrDefault().Name;

            var updateReq = _securityTestUtilities.Permission.ConvertPermissionDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Name = dupeName;

            // Act
            var updateResult = await _permissionLogic.Update(recordToUpdate.PermissionId, updateReq, _applicationLogic);

            //Assert
            var expectedUniqueNameError = _securityTestUtilities.Permission.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueNameError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.Permission.CreateActiveTestRecords(applicationId);
            var testRecords = await _permissionLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _permissionLogic.Update(recordToUpdate.PermissionId, new InsertUpdatePermissionRequest(), _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        //public async Task Permission_Insert_Should_Not_Create_Record_Invalid_ApplicationId_Error()

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.Permission.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(applicationId);

            // Act
            var result = await _permissionLogic.Delete(testRecord.PermissionId);
            var getResult = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _permissionLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
