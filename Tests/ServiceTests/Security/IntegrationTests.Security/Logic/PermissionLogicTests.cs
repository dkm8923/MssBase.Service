using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;
using System.Text.Json;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class PermissionLogicTests : SecurityTestBase, 
                                        IDefaultLogicTestsGetAll,
                                        IDefaultLogicTestsGetAllReadOnly,
                                        IDefaultLogicTestsGetById,
                                        IDefaultLogicTestsGetByIdReadOnly,
                                        IDefaultLogicTestsGetAuditLogsById,
                                        IDefaultLogicTestsFilter,
                                        IDefaultLogicTestsFilterReadOnly,  
                                        IDefaultLogicTestsInsert, 
                                        IDefaultLogicTestsUpdate,
                                        IDefaultLogicTestsUpdateReadOnly,
                                        IDefaultLogicTestsDelete,
                                        IDefaultLogicTestsDeleteReadOnly
    {
        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            
            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet());

            // Assert
            var recordCt = arrangeTestDataResponse.ActivePermissions.Count();
            result.Response.Should().HaveCount(recordCt);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            var recordCt = arrangeTestDataResponse.ActivePermissions.Count() + arrangeTestDataResponse.InactivePermissions.Count();
            result.Response.Should().HaveCount(recordCt);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true });

            // Assert
            result.Response.Should().HaveCount(1);
            
            foreach (var record in result.Response)
            {
                record.ReadOnly.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true, IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(2);

            foreach (var record in result.Response)
            {
                record.ReadOnly.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_ReadOnly_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);

            // Act
            var result = await _permissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(2);

            foreach (var record in result.Response)
            {
                record.ReadOnly.Should().BeFalse();
            }
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            var testRecord = arrangeTestDataResponse.ActivePermissions.FirstOrDefault();
            
            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            var testRecord = arrangeTestDataResponse.InactivePermissions.FirstOrDefault();

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            var testRecord = arrangeTestDataResponse.InactivePermissions.FirstOrDefault();

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];
            
            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet { IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];
    
            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];

            // Act
            var result = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        #endregion

        #region Get Audit Logs By Id

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).First();
            
            var updateReq = _securityTestUtilities.Permission.ConvertPermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.Name = "Updated Permission Name";
            updateReq.Description = "Updated Permission Description";
            updateReq.Active = false;

            // Act
            var updateResult = await _permissionLogic.Update(testRecord.PermissionId, updateReq, _applicationLogic);
            var auditLogResult = await _permissionLogic.GetAuditLogsByPermissionId(testRecord.PermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeUpdate);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypePermission);
            res.ReferenceId.Should().Be(testRecord.PermissionId);

            var changeLog = ((JsonElement)res.ChangeLogJson).Deserialize<PermissionChangeLog>();
            changeLog.Should().NotBeNull();
            changeLog.Name.Should().Be(updateReq.Name);
            changeLog.Description.Should().Be(updateReq.Description);
            changeLog.Active.Should().Be(updateReq.Active);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<PermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.PermissionId = res.ReferenceId;

            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).First();

            // Act
            await _permissionLogic.Delete(testRecord.PermissionId, TestConstants.CurrentUser);
            var getResult = await _permissionLogic.GetById(testRecord.PermissionId, new BaseLogicGet());
            var auditLogResult = await _permissionLogic.GetAuditLogsByPermissionId(testRecord.PermissionId);

            // Assert
            getResult.Response.Should().BeNull();

            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeDelete);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypePermission);
            res.ReferenceId.Should().Be(testRecord.PermissionId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<PermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.PermissionId = res.ReferenceId;

            _securityTestUtilities.Permission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).First();

            var updateReq = _securityTestUtilities.Permission.ConvertPermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.Name = "Updated Permission Name";

            // Act
            var updateResult = await _permissionLogic.Update(testRecord.PermissionId, updateReq, _applicationLogic);
            await _permissionLogic.Delete(testRecord.PermissionId, TestConstants.CurrentUser);
            var auditLogResult = await _permissionLogic.GetAuditLogsByPermissionId(testRecord.PermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(2);

            var updateRes = auditLogResult.Response.First();
            updateRes.LogType.Should().Be(TestConstants.LogTypeUpdate);
            updateRes.ReferenceType.Should().Be(TestConstants.ReferenceTypePermission);
            updateRes.ReferenceId.Should().Be(testRecord.PermissionId);

            var deleteRes = auditLogResult.Response.Last();
            deleteRes.LogType.Should().Be(TestConstants.LogTypeDelete);
            deleteRes.ReferenceType.Should().Be(TestConstants.ReferenceTypePermission);
            deleteRes.ReferenceId.Should().Be(testRecord.PermissionId);
        }

        class PermissionChangeLog
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool? Active { get; set; }
            public string? UpdatedBy { get; set; }
            public DateTime? UpdatedOn { get; set; }
        }

        #endregion 

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();

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
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);

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
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);

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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            int applicationId = arrangeTestDataResponse.ActivePermissions.FirstOrDefault()?.ApplicationId ?? 0;
            var permissions = arrangeTestDataResponse.ActivePermissions;

            //create test permissions for filtering tests
            var testPermission1 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 1",
                Description = "Test Permission Description 1",
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic);

            var testPermission2 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 2",
                Description = "Test Permission Description 2",
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic);

            await _permissionLogic.Update(testPermission2.Response.PermissionId, new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 2",
                Description = "Test Permission Description 2",
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            }, _applicationLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterPermissionLogicRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterPermissionLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterPermissionLogicRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
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

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

           var postReq = new FilterPermissionLogicRequest { IncludeReadOnly = true };

            // Act
            var result = await _permissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);
            
            foreach (var r in result.Response)
            {
                r.Active.Should().BeTrue();
                r.ReadOnly.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Inactive_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

            var postReq = new FilterPermissionLogicRequest { IncludeInactive = true, IncludeReadOnly = true };

            // Act
            var result = await _permissionLogic.Filter(postReq);

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
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = (await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1)).First();
            await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);
            
            var postReqInvalidCreatedBy = new FilterPermissionLogicRequest { CreatedBy = testRecord.CreatedBy };
            var postReqInvalidCreatedOnDate = new FilterPermissionLogicRequest { CreatedOnDate = DateOnly.FromDateTime(testRecord.CreatedOn) };
            var postReqInvalidUpdatedBy = new FilterPermissionLogicRequest { UpdatedBy = testRecord.UpdatedBy };
            var postReqInvalidUpdatedOnDate = new FilterPermissionLogicRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)testRecord.UpdatedOn) };
            var postReqInvalidName = new FilterPermissionLogicRequest { Name = testRecord.Name };
            
            // Act
            var invalidCreatedByResult = await _permissionLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _permissionLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _permissionLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _permissionLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidNameResult = await _permissionLogic.Filter(postReqInvalidName);
            
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
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            int applicationId = arrangeTestDataResponse.ActivePermissions.FirstOrDefault()?.ApplicationId ?? 0;
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
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            int applicationId = arrangeTestDataResponse.ActivePermissions.FirstOrDefault()?.ApplicationId ?? 0;
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
            await ClearAllSecurityTestTableData();
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
            await ClearAllSecurityTestTableData();
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
            var arrangeTestDataResponse = await ArrangePermissionTestData(1, 1);
            int applicationId = arrangeTestDataResponse.ActivePermissions.FirstOrDefault()?.ApplicationId ?? 0;
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "Updated name",
                Description = "Updated description",
                Active = false,
                ApplicationId = newApplication.ApplicationId,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecords = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
            var testRecords = await _permissionLogic.GetAll(new BaseLogicGet());
            var recordToUpdate = testRecords.Response.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _permissionLogic.Update(recordToUpdate.PermissionId, new InsertUpdatePermissionRequest(), _applicationLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToUpdate = (await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1)).First();

            var updateReq = _securityTestUtilities.Permission.ConvertPermissionDtoToInsertUpdateRequest(recordToUpdate);
            
            // Act
            var updateResult = await _permissionLogic.Update(recordToUpdate.PermissionId, updateReq, _applicationLogic);

            //Assert
            var expectedReadOnlyError = _securityTestUtilities.Permission.GetExpectedReadOnlyErrors();

            //Assert
            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedReadOnlyError);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);

            // Act
            var result = await _permissionLogic.Delete(testRecord.PermissionId, TestConstants.CurrentUser);
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
            var result = await _permissionLogic.Delete(-1, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = (await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1)).First();

            var expectedFieldErrors = _securityTestUtilities.Permission.GetExpectedReadOnlyErrors();

            // Act
            var result = await _permissionLogic.Delete(testRecord.PermissionId, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
