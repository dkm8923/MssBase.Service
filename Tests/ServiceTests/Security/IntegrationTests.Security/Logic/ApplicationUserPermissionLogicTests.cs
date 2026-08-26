using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Dto.Security.ApplicationUserPermission.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;
using Dto.Security.Permission;
using System.Text.Json;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserPermissionLogicTests : SecurityTestBase,
                                                       IDefaultLogicTestsGetAll,
                                                       IDefaultLogicTestsGetAllIncludeRelated,
                                                       IDefaultLogicTestsGetAllReadOnly,
                                                       IDefaultLogicTestsGetById,
                                                       IDefaultLogicTestsGetByIdIncludeRelated,
                                                       IDefaultLogicTestsGetByIdReadOnly,
                                                       IDefaultLogicTestsGetAuditLogsById,
                                                       IDefaultLogicTestsFilter,
                                                       IDefaultLogicTestsFilterIncludeRelated,
                                                       IDefaultLogicTestsFilterReadOnly,  
                                                       IDefaultLogicTestsInsert, 
                                                       IDefaultLogicTestsUpdate,
                                                       IDefaultLogicTestsUpdateReadOnly,
                                                       IDefaultLogicTestsDelete,
                                                       IDefaultLogicTestsDeleteReadOnly
    {
        #region Private

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var activeResult = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet());
            var inactiveResult = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            activeResult.Response.Should().HaveCount(0);
            inactiveResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().NotBeNull();
                applicationUserPermission.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet());
            
            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var applicationUserPermission = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true });

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
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permissions =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 2);
            await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[0].PermissionId);
            await _securityTestUtilities.ApplicationUserPermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[1].PermissionId);

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true, IncludeInactive = true });

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
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permissions =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 3);
            
            await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(application.ApplicationId, applicationUser.ApplicationUserId, permissions[0].PermissionId, 1);
            await _securityTestUtilities.ApplicationUserPermission.CreateInactiveTestRecords(application.ApplicationId, applicationUser.ApplicationUserId, permissions[1].PermissionId, 1);
            await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[2].PermissionId);
            
            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());
            var resultWithIncludeInactiveFalse = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = false });

            // Assert
            result.Response.Should().BeNull();
            resultWithIncludeInactiveFalse.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();
            result.Response.Permission.Active.Should().BeTrue();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);
    
            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var applicationUserPermission = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            var postReq = new FilterApplicationUserPermissionServiceRequest { IncludeReadOnly = true };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

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

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permissions =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 2);
            await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[0].PermissionId);
            await _securityTestUtilities.ApplicationUserPermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[1].PermissionId);

            var postReq = new FilterApplicationUserPermissionServiceRequest { IncludeInactive = true, IncludeReadOnly = true };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

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

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permissions =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 2);
            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permissions[0].PermissionId);
            
            var postReqInvalidCreatedBy = new FilterApplicationUserPermissionLogicRequest { CreatedBy = testRecord.CreatedBy };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserPermissionLogicRequest { CreatedOnDate = DateOnly.FromDateTime(testRecord.CreatedOn) };
            var postReqInvalidUpdatedBy = new FilterApplicationUserPermissionLogicRequest { UpdatedBy = testRecord.UpdatedBy };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserPermissionLogicRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)testRecord.UpdatedOn) };
            var postReqInvalidApplicationUserPermissionIds = new FilterApplicationUserPermissionLogicRequest { ApplicationUserPermissionIds = new List<int> { testRecord.ApplicationUserPermissionId } };
            var postReqInvalidApplicationId = new FilterApplicationUserPermissionLogicRequest { ApplicationId = testRecord.ApplicationId };
            var postReqInvalidApplicationUserId = new FilterApplicationUserPermissionLogicRequest { ApplicationUserId = testRecord.ApplicationUserId };
            var postReqInvalidPermissionId = new FilterApplicationUserPermissionLogicRequest { PermissionId = testRecord.PermissionId };

            // Act
            var invalidCreatedByResult = await _applicationUserPermissionLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationUserPermissionLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidApplicationUserPermissionIdsResult = await _applicationUserPermissionLogic.Filter(postReqInvalidApplicationUserPermissionIds);
            var invalidApplicationIdResult = await _applicationUserPermissionLogic.Filter(postReqInvalidApplicationId);
            var invalidApplicationUserIdResult = await _applicationUserPermissionLogic.Filter(postReqInvalidApplicationUserId);
            var invalidPermissionIdResult = await _applicationUserPermissionLogic.Filter(postReqInvalidPermissionId);

            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserPermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdResult.Response.Should().HaveCount(0);
            invalidPermissionIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Get Audit Logs By Id

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newApplicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(newApplication.ApplicationId);
            var newPermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(newApplication.ApplicationId);

            var updateReq = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.ApplicationUserId = newApplicationUser.ApplicationUserId;
            updateReq.PermissionId = newPermission.PermissionId;

            // Act
            var updateResult = await _applicationUserPermissionLogic.Update(testRecord.ApplicationUserPermissionId, updateReq, _applicationLogic, _applicationUserLogic, _permissionLogic);
            var auditLogResult = await _applicationUserPermissionLogic.GetAuditLogsByApplicationUserPermissionId(testRecord.ApplicationUserPermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeUpdate);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUserPermission);
            res.ReferenceId.Should().Be(testRecord.ApplicationUserPermissionId);

            var changeLog = ((JsonElement)res.ChangeLogJson).Deserialize<ApplicationUserPermissionChangeLog>();
            changeLog.Should().NotBeNull();
            changeLog.ApplicationId.Should().Be(updateReq.ApplicationId);
            changeLog.ApplicationUserId.Should().Be(updateReq.ApplicationUserId);
            changeLog.PermissionId.Should().Be(updateReq.PermissionId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<ApplicationUserPermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.ApplicationUserPermissionId = res.ReferenceId;

            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId));
            
            // Act
            await _applicationUserPermissionLogic.Delete(testRecord.ApplicationUserPermissionId, TestConstants.CurrentUser);
            var getResult = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());
            var auditLogResult = await _applicationUserPermissionLogic.GetAuditLogsByApplicationUserPermissionId(testRecord.ApplicationUserPermissionId);

            // Assert
            getResult.Response.Should().BeNull();

            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeDelete);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUserPermission);
            res.ReferenceId.Should().Be(testRecord.ApplicationUserPermissionId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<ApplicationUserPermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.ApplicationUserPermissionId = res.ReferenceId;

            _securityTestUtilities.ApplicationUserPermission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newApplicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(newApplication.ApplicationId);
            var newPermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(newApplication.ApplicationId);

            var updateReq = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.ApplicationUserId = newApplicationUser.ApplicationUserId;
            updateReq.PermissionId = newPermission.PermissionId;

            // Act
            var updateResult = await _applicationUserPermissionLogic.Update(testRecord.ApplicationUserPermissionId, updateReq, _applicationLogic, _applicationUserLogic, _permissionLogic);
            await _applicationUserPermissionLogic.Delete(testRecord.ApplicationUserPermissionId, TestConstants.CurrentUser);
            var auditLogResult = await _applicationUserPermissionLogic.GetAuditLogsByApplicationUserPermissionId(testRecord.ApplicationUserPermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(2);

            var updateRes = auditLogResult.Response.First();
            updateRes.LogType.Should().Be(TestConstants.LogTypeUpdate);
            updateRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUserPermission);
            updateRes.ReferenceId.Should().Be(testRecord.ApplicationUserPermissionId);

            var deleteRes = auditLogResult.Response.Last();
            deleteRes.LogType.Should().Be(TestConstants.LogTypeDelete);
            deleteRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUserPermission);
            deleteRes.ReferenceId.Should().Be(testRecord.ApplicationUserPermissionId);
        }

        class ApplicationUserPermissionChangeLog
        {
            public int? ApplicationId { get; set; }
            public int? ApplicationUserId { get; set; }
            public int? PermissionId { get; set; }
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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReq = new FilterApplicationUserPermissionLogicRequest { };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReq = new FilterApplicationUserPermissionLogicRequest { IncludeInactive = true };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);

            result.Response.Where(r => r.Active).ToList().Should().HaveCountGreaterThan(0);
            result.Response.Where(r => !r.Active).ToList().Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var applicationUserPermission = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();
            var applicationId = applicationUserPermission.ApplicationId;
            var applicationUserId = applicationUserPermission.ApplicationUserId;
            var permissionId = applicationUserPermission.PermissionId;
            
            //create new permission
            var testPermission1 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 1",
                Description = "Test Permission Description 1",
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            }, _applicationLogic);

            //create new application user permission with specific created / updated by values
            var testApplicationUserPermission1Res = await _applicationUserPermissionLogic.Insert(new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = applicationId,
                ApplicationUserId = applicationUserId,
                PermissionId = testPermission1.Response.PermissionId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic, _applicationUserLogic, _permissionLogic);

            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newApplicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(newApplication.ApplicationId);
            var newPermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(newApplication.ApplicationId);

            await _applicationUserPermissionLogic.Update(testApplicationUserPermission1Res.Response.ApplicationUserPermissionId, new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = newApplication.ApplicationId,
                ApplicationUserId = newApplicationUser.ApplicationUserId,
                PermissionId = newPermission.PermissionId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            }, _applicationLogic, _applicationUserLogic, _permissionLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationUserPermissionServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterApplicationUserPermissionServiceRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationUserPermissionServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
            var postReqFilterUpdatedOnDate = new FilterApplicationUserPermissionServiceRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationUserPermissionIds = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = arrangeTestDataResponse.ActiveApplicationUserPermissions.Select(x => x.ApplicationUserPermissionId).ToList() };
            var postReqFilterApplicationId = new FilterApplicationUserPermissionServiceRequest { ApplicationId = applicationId };
            var postReqFilterPermissionId = new FilterApplicationUserPermissionServiceRequest { PermissionId = permissionId };
            
            // Act
            var filterCreatedByResult = await _applicationUserPermissionLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _applicationUserPermissionLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqFilterUpdatedOnDate);
            var filterApplicationUserPermissionIdsResult = await _applicationUserPermissionLogic.Filter(postReqFilterApplicationUserPermissionIds);
            var filterApplicationIdResult = await _applicationUserPermissionLogic.Filter(postReqFilterApplicationId);
            var filterPermissionIdResult = await _applicationUserPermissionLogic.Filter(postReqFilterPermissionId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(1);
            filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterApplicationUserPermissionIdsResult.Response.Should().HaveCountGreaterThan(0);
            filterApplicationIdResult.Response.Should().HaveCountGreaterThan(0);
            filterPermissionIdResult.Response.Should().HaveCount(1);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReq = new FilterApplicationUserPermissionLogicRequest { IncludeRelated = true };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Active.Should().BeTrue();
                applicationUserPermission.Permission.Should().NotBeNull();
                applicationUserPermission.Permission.Active.Should().BeTrue();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReq = new FilterApplicationUserPermissionLogicRequest { IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().NotBeNull();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReq = new FilterApplicationUserPermissionLogicRequest();

            // Act
            var result = await _applicationUserPermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUserPermission in result.Response)
            {
                applicationUserPermission.Permission.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();

            var postReqInvalidCreatedBy = new FilterApplicationUserPermissionServiceRequest { CreatedBy = "asdfasdf" };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserPermissionServiceRequest { CreatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidUpdatedBy = new FilterApplicationUserPermissionServiceRequest { UpdatedBy = "asdfasdf" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserPermissionServiceRequest { UpdatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidApplicationUserPermissionIds = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { -1 } };
            var postReqInvalidApplicationId = new FilterApplicationUserPermissionServiceRequest { ApplicationId = -1 };
            var postReqInvalidPermissionId = new FilterApplicationUserPermissionServiceRequest { PermissionId = -1 };
            
            // Act
            var invalidCreatedByResult = await _applicationUserPermissionLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationUserPermissionLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationUserPermissionLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidApplicationUserPermissionIdsResult = await _applicationUserPermissionLogic.Filter(postReqInvalidApplicationUserPermissionIds);
            var invalidApplicationIdResult = await _applicationUserPermissionLogic.Filter(postReqInvalidApplicationId);
            var invalidPermissionIdResult = await _applicationUserPermissionLogic.Filter(postReqInvalidPermissionId);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserPermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidPermissionIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        //securityTestData

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateActiveTestRecords(1);
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application[0].ApplicationId, 1);
            var permission =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application[0].ApplicationId, 1);

            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application[0].ApplicationId,
                ApplicationUserId = applicationUser[0].ApplicationUserId,
                PermissionId = permission[0].PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserPermissionLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.ApplicationId.Should().Be(insertReq.ApplicationId);
            result.Response.ApplicationUserId.Should().Be(insertReq.ApplicationUserId);
            result.Response.PermissionId.Should().Be(insertReq.PermissionId);
            result.Response.Active.Should().BeTrue();
            result.Response.CreatedBy.Should().Be(TestConstants.CurrentUser);
            result.Response.UpdatedBy.Should().Be(TestConstants.CurrentUser);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var applicationUserPermission = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();
            var recordToCreate = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(applicationUserPermission);

            var expectedUniqueError = _securityTestUtilities.ApplicationUserPermission.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _permissionLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var recordToCreate = new InsertUpdateApplicationUserPermissionRequest();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _permissionLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.ApplicationUserPermission.CreateInsertUpdateRequestWithMaxLengthErrors(1, 1, 1);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _permissionLogic);

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();   

            var updateReq = new InsertUpdateApplicationUserPermissionRequest
            {
                Active = false,
                ApplicationId = recordToUpdate.ApplicationId,
                ApplicationUserId = recordToUpdate.ApplicationUserId,
                PermissionId = recordToUpdate.PermissionId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserPermissionLogic.Update(recordToUpdate.ApplicationUserPermissionId, updateReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            result.Response.ApplicationUserId.Should().Be(updateReq.ApplicationUserId);
            result.Response.PermissionId.Should().Be(updateReq.PermissionId);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();   
            
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(arrangeTestDataResponse.ActiveApplications[0].ApplicationId);
            var activePermission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, 1)).FirstOrDefault();
            var recordToCopy = (await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId, 1)).FirstOrDefault();
            
            var updateReq = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.ApplicationId = recordToCopy.ApplicationId;
            updateReq.ApplicationUserId = recordToCopy.ApplicationUserId;
            updateReq.PermissionId = recordToCopy.PermissionId;

            // Act
            var updateResult = await _applicationUserPermissionLogic.Update(recordToUpdate.ApplicationUserPermissionId, updateReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

            //Assert
            var expectedUniqueApplicationuserPermissionError = _securityTestUtilities.ApplicationUserPermission.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueApplicationuserPermissionError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Update(recordToUpdate.ApplicationUserPermissionId, new InsertUpdateApplicationUserPermissionRequest(), _applicationLogic, _applicationUserLogic, _permissionLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var recordToUpdate = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            var updateReq = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(recordToUpdate);
            
            // Act
            var updateResult = await _applicationUserPermissionLogic.Update(recordToUpdate.ApplicationUserPermissionId, updateReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

            //Assert
            var expectedReadOnlyError = _securityTestUtilities.ApplicationUserPermission.GetExpectedReadOnlyErrors();

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
            var arrangeTestDataResponse = await ArrangeApplicationUserPermissionTestData();
            var recordToDelete = arrangeTestDataResponse.ActiveApplicationUserPermissions.FirstOrDefault();   

            // Act
            var result = await _applicationUserPermissionLogic.Delete(recordToDelete.ApplicationUserPermissionId, TestConstants.CurrentUser);
            var getResult = await _applicationUserPermissionLogic.GetById(recordToDelete.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Delete(-1, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var permission =  (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedReadOnlyErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Delete(testRecord.ApplicationUserPermissionId, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
