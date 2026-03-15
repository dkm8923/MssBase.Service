using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Security.Application.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;
using IntegrationTests.Shared.Utilities.Contracts.Logic;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationLogicTests : SecurityTestBase, 
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
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _applicationLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _applicationLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(6);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await _applicationLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
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
            var result = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            var result = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Unused_Id_Should_Return_Null()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            var id = -1;

            // Act
            var result = await _applicationLogic.GetById(id, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().BeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();

            var postReq = new FilterApplicationLogicRequest { };

            // Act
            var result = await _applicationLogic.Filter(postReq);

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
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            var postReq = new FilterApplicationLogicRequest { IncludeInactive = true };

            // Act
            var result = await _applicationLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0); //activeRecords
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0); //inactiveRecords
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Data()
        {
            //TODO: Test filtering by multiple application ids
            // Arrange
            await ClearAllSecurityTestTableData();
            var testData = await _securityTestUtilities.Application.CreateActiveTestRecords();

            var testRecord = testData.FirstOrDefault();

            var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser };
            var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = TestConstants.CurrentUser };
            var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var postReqName = new FilterApplicationServiceRequest { Name = testRecord.Name };
            
            // Act
            var filterCreatedByResult = await _applicationLogic.Filter(postReqCreatedBy);
            var filterCreatedOnDateResult = await _applicationLogic.Filter(postReqCreatedOnDate);
            var filterUpdatedByResult = await _applicationLogic.Filter(postReqUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationLogic.Filter(postReqUpdatedOnDate);
            var filterNameResult = await _applicationLogic.Filter(postReqName);
            
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
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();

            var postReqInvalidCreatedBy = new FilterApplicationServiceRequest { CreatedBy = "TestCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = "TestUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse("1/1/2000") };
            var postReqInvalidName = new FilterApplicationServiceRequest { Name = "TestApplicationName" };
            
            // Act
            var invalidCreatedByResult = await _applicationLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidNameResult = await _applicationLogic.Filter(postReqInvalidName);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidNameResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var applications = await _securityTestUtilities.Application.CreateActiveTestRecords();

            //create test roles for filtering tests
            var testApplication1 = await _applicationLogic.Insert(new InsertUpdateApplicationRequest
            {
                Active = true,
                Name = "Test Application Name 1",
                Description = "Test Application Description 1",
                CurrentUser = "IntegrationTestInsert"
            });

            var testApplication2 = await _applicationLogic.Insert(new InsertUpdateApplicationRequest
            {
                Active = true,
                Name = "Test Application Name 2",
                Description = "Test Application Description 2",
                CurrentUser = "IntegrationTestInsert"
            });

            await _applicationLogic.Update(testApplication2.Response.ApplicationId, new InsertUpdateApplicationRequest
            {
                Active = true,
                Name = "Test Application Name 2",
                Description = "Test Application Description 2",
                CurrentUser = "IntegrationTestUpdate"
            });

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationLogicRequest { CreatedBy = "IntegrationTestInsert" };
            var postReqFilterCreatedOnDate = new FilterApplicationLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationLogicRequest { UpdatedBy = "IntegrationTestUpdate" };
            var postReqFilterUpdatedOnDate = new FilterApplicationLogicRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationIds = new FilterApplicationLogicRequest { ApplicationIds = new List<int> { applications[0].ApplicationId, applications[1].ApplicationId, applications[2].ApplicationId } };
            var postReqFilterName = new FilterApplicationLogicRequest { Name = testApplication1.Response.Name };
            
            // Act
            var filterCreatedByResult = await _applicationLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _applicationLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _applicationLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationLogic.Filter(postReqFilterUpdatedOnDate);
            var filterApplicationIdsResult = await _applicationLogic.Filter(postReqFilterApplicationIds);
            var filterNameResult = await _applicationLogic.Filter(postReqFilterName);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(2);
            filterCreatedOnDateResult.Response.Should().HaveCount(7);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(7);
            filterApplicationIdsResult.Response.Should().HaveCount(3);
            filterNameResult.Response.Should().HaveCount(1);
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

            var insertCheck = await _applicationLogic.GetById(insertedRecord.ApplicationId, new BaseLogicGet());

            //Assert
            _securityTestUtilities.Application.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var recordToCreate = _securityTestUtilities.Application.ConvertApplicationDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueApplicationNameError = _securityTestUtilities.Application.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _applicationLogic.Insert(recordToCreate);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueApplicationNameError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = new InsertUpdateApplicationRequest();

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationLogic.Insert(recordToCreate);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.Application.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationLogic.Insert(recordToCreate);

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
            var updateResult = await _applicationLogic.Update(insertedRecord.ApplicationId, updateReq);

            //Assert
            updateResult.Response.ApplicationId.Should().Be(insertedRecord.ApplicationId);
            updateResult.Response.Name.Should().Be(updateReq.Name);
            updateResult.Response.Description.Should().Be(updateReq.Description);
            updateResult.Response.Active.Should().Be(updateReq.Active);
            updateResult.Response.CreatedOn.Should().NotBe(updateResult.Response.UpdatedOn);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecords = await _securityTestUtilities.Application.CreateActiveTestRecords();
            var recordToUpdate = testRecords.FirstOrDefault();
            var dupeApplicationName = testRecords.LastOrDefault().Name;

            var updateReq = _securityTestUtilities.Application.ConvertApplicationDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Name = dupeApplicationName;

            // Act
            var updateResult = await _applicationLogic.Update(recordToUpdate.ApplicationId, updateReq);

            //Assert
            var expectedUniqueUserFriendlyDescriptionError = _securityTestUtilities.Application.GetExpectedUniqueFieldErrors();

            //Assert
            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueUserFriendlyDescriptionError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            var testRecords = await _applicationLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationLogic.Update(recordToUpdate.ApplicationId, new InsertUpdateApplicationRequest());

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            var testRecords = await _applicationLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var updateReq = _securityTestUtilities.Application.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationLogic.Update(recordToUpdate.ApplicationId, updateReq);

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
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

            // Act
            await _applicationLogic.Delete(testRecord.ApplicationId);
            var getResult = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet());

            // Assert
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _applicationLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Application_Delete_Should_Not_Delete_Record_ApplicationUser_Foreign_Key_Dependency_Exists()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            //get application user dependency error
            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedForeignKeyErrors();
            expectedFieldErrors = expectedFieldErrors.Where(x => x.Key == "ApplicationUsers").ToDictionary();

            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            //create test application user
            await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(testRecord.ApplicationId);
            
            // Act
            var result = await _applicationLogic.Delete(testRecord.ApplicationId);
            
            // Assert
            result.Errors.Count.Should().Be(1);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Application_Delete_Should_Not_Delete_Record_Permission_Foreign_Key_Dependency_Exists()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            //get permission dependency error
            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedForeignKeyErrors();
            expectedFieldErrors = expectedFieldErrors.Where(x => x.Key == "Permissions").ToDictionary();

            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            //create test permission
            await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(testRecord.ApplicationId);
            
            // Act
            var result = await _applicationLogic.Delete(testRecord.ApplicationId);
            
            // Assert
            result.Errors.Count.Should().Be(1);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Application_Delete_Should_Not_Delete_Record_Role_Foreign_Key_Dependency_Exists()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            //get role dependency error
            var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedForeignKeyErrors();
            expectedFieldErrors = expectedFieldErrors.Where(x => x.Key == "Roles").ToDictionary();

            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            //create test role
            await _securityTestUtilities.Role.CreateSingleRoleTestRecord(testRecord.ApplicationId);
            
            // Act
            var result = await _applicationLogic.Delete(testRecord.ApplicationId);
            
            // Assert
            result.Errors.Count.Should().Be(1);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        // [Fact]
        // public async Task Application_Delete_Should_Not_Delete_Record_RolePermission_Foreign_Key_Dependency_Exists()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();

        //     //get role permission dependency error
        //     var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedForeignKeyErrors();
        //     expectedFieldErrors = expectedFieldErrors.Where(x => x.Key == "RolePermissions").ToDictionary();

        //     var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        //     //create test role permission
        //     await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(testRecord.ApplicationId);
            
        //     // Act
        //     var result = await _applicationLogic.Delete(testRecord.ApplicationId);
            
        //     // Assert
        //     result.Errors.Count.Should().Be(1);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        // }

        // [Fact]
        // public async Task Application_Delete_Should_Not_Delete_Record_ApplicationUserPermission_Foreign_Key_Dependency_Exists()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();

        //     //get permission dependency error
        //     var expectedFieldErrors = _securityTestUtilities.Application.GetExpectedForeignKeyErrors();
        //     expectedFieldErrors = expectedFieldErrors.Where(x => x.Key == "ApplicationUserPermissions").ToDictionary();

        //     var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        //     //create test application user permission
        //     await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(testRecord.ApplicationId);
            
        //     // Act
        //     var result = await _applicationLogic.Delete(testRecord.ApplicationId);
            
        //     // Assert
        //     result.Errors.Count.Should().Be(1);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        // }

        #endregion
    }
}
