using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Logic;
using Dto.Security.RolePermission.Service;
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
    public class RolePermissionLogicTests : SecurityTestBase,
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var activeResult = await _rolePermissionLogic.GetAll(new BaseLogicGet());
            var inactiveResult = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            activeResult.Response.Should().HaveCount(0);
            inactiveResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().HaveCount(5);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().NotBeNull();
                rolePermission.Permission.Active.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(10);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);

            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var rolePermission = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true });

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 2)).FirstOrDefault();
            var roles =  await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 2);
            await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, roles[0].RoleId, permission.PermissionId);
            await _securityTestUtilities.RolePermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, roles[1].RoleId, permission.PermissionId);

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true, IncludeInactive = true });

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var roles =  await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 3);
            
            await _securityTestUtilities.RolePermission.CreateActiveTestRecords(application.ApplicationId, roles[0].RoleId, permission.PermissionId, 1);
            await _securityTestUtilities.RolePermission.CreateInactiveTestRecords(application.ApplicationId, roles[1].RoleId, permission.PermissionId, 1);
            await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, roles[2].RoleId, permission.PermissionId);
            
            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());
            var resultWithIncludeInactiveFalse = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeInactive = false });

            // Assert
            result.Response.Should().BeNull();
            resultWithIncludeInactiveFalse.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeRelated = true });

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.InactiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Permission.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var testRecord = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.RolePermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);
    
            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(newApplication.ApplicationId);
            var newPermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(newApplication.ApplicationId);

            var updateReq = _securityTestUtilities.RolePermission.ConvertRolePermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.RoleId = newRole.RoleId;
            updateReq.PermissionId = newPermission.PermissionId;

            // Act
            var updateResult = await _rolePermissionLogic.Update(testRecord.RolePermissionId, updateReq, _applicationLogic, _roleLogic, _permissionLogic);
            var auditLogResult = await _rolePermissionLogic.GetAuditLogsByRolePermissionId(testRecord.RolePermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeUpdate);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeRolePermission);
            res.ReferenceId.Should().Be(testRecord.RolePermissionId);

            var changeLog = ((JsonElement)res.ChangeLogJson).Deserialize<RolePermissionChangeLog>();
            changeLog.Should().NotBeNull();
            changeLog.ApplicationId.Should().Be(updateReq.ApplicationId);
            changeLog.RoleId.Should().Be(updateReq.RoleId);
            changeLog.PermissionId.Should().Be(updateReq.PermissionId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<RolePermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.RolePermissionId = res.ReferenceId;

            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId));
            
            // Act
            await _rolePermissionLogic.Delete(testRecord.RolePermissionId, TestConstants.CurrentUser);
            var getResult = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());
            var auditLogResult = await _rolePermissionLogic.GetAuditLogsByRolePermissionId(testRecord.RolePermissionId);

            // Assert
            getResult.Response.Should().BeNull();

            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeDelete);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeRolePermission);
            res.ReferenceId.Should().Be(testRecord.RolePermissionId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<RolePermissionDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.RolePermissionId = res.ReferenceId;

            _securityTestUtilities.RolePermission.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var testRecord = (await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newRole = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(newApplication.ApplicationId);
            var newPermission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(newApplication.ApplicationId);

            var updateReq = _securityTestUtilities.RolePermission.ConvertRolePermissionDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.RoleId = newRole.RoleId;
            updateReq.PermissionId = newPermission.PermissionId;

            // Act
            var updateResult = await _rolePermissionLogic.Update(testRecord.RolePermissionId, updateReq, _applicationLogic, _roleLogic, _permissionLogic);
            await _rolePermissionLogic.Delete(testRecord.RolePermissionId, TestConstants.CurrentUser);
            var auditLogResult = await _rolePermissionLogic.GetAuditLogsByRolePermissionId(testRecord.RolePermissionId);

            // Assert
            auditLogResult.Response.Should().HaveCount(2);

            var updateRes = auditLogResult.Response.First();
            updateRes.LogType.Should().Be(TestConstants.LogTypeUpdate);
            updateRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeRolePermission);
            updateRes.ReferenceId.Should().Be(testRecord.RolePermissionId);

            var deleteRes = auditLogResult.Response.Last();
            deleteRes.LogType.Should().Be(TestConstants.LogTypeDelete);
            deleteRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeRolePermission);
            deleteRes.ReferenceId.Should().Be(testRecord.RolePermissionId);
        }

        class RolePermissionChangeLog
        {
            public int? ApplicationId { get; set; }
            public int? RoleId { get; set; }
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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReq = new FilterRolePermissionLogicRequest { };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReq = new FilterRolePermissionLogicRequest { IncludeInactive = true };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var rolePermission = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();
            var applicationId = rolePermission.ApplicationId;
            var roleId = rolePermission.RoleId;
            var permissionId = rolePermission.PermissionId;
            
            //create new permission
            var testPermission1 = await _permissionLogic.Insert(new InsertUpdatePermissionRequest
            {
                ApplicationId = applicationId,
                Name = "Test Permission Name 1",
                Description = "Test Permission Description 1",
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            }, _applicationLogic);

            //create new role permission with specific created / updated by values
            var testRolePermission1Res = await _rolePermissionLogic.Insert(new InsertUpdateRolePermissionRequest
            {
                ApplicationId = applicationId,
                RoleId = roleId,
                PermissionId = testPermission1.Response.PermissionId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic, _roleLogic, _permissionLogic);

            await _rolePermissionLogic.Update(testRolePermission1Res.Response.RolePermissionId, new InsertUpdateRolePermissionRequest
            {
                ApplicationId = applicationId,
                RoleId = roleId,
                PermissionId = testPermission1.Response.PermissionId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            }, _applicationLogic, _roleLogic, _permissionLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterRolePermissionServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterRolePermissionServiceRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterRolePermissionServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
            var postReqFilterUpdatedOnDate = new FilterRolePermissionServiceRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { arrangeTestDataResponse.ActiveRolePermissions[0].RolePermissionId, arrangeTestDataResponse.ActiveRolePermissions[1].RolePermissionId, arrangeTestDataResponse.ActiveRolePermissions[2].RolePermissionId } };
            var postReqFilterApplicationId = new FilterRolePermissionServiceRequest { ApplicationId = applicationId };
            var postReqFilterPermissionId = new FilterRolePermissionServiceRequest { PermissionId = permissionId };
            
            // Act
            var filterCreatedByResult = await _rolePermissionLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _rolePermissionLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _rolePermissionLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _rolePermissionLogic.Filter(postReqFilterUpdatedOnDate);
            var filterRolePermissionIdsResult = await _rolePermissionLogic.Filter(postReqFilterRolePermissionIds);
            var filterApplicationIdResult = await _rolePermissionLogic.Filter(postReqFilterApplicationId);
            var filterPermissionIdResult = await _rolePermissionLogic.Filter(postReqFilterPermissionId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(1);
            filterCreatedOnDateResult.Response.Should().HaveCount(6);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(6);
            filterRolePermissionIdsResult.Response.Should().HaveCount(3);
            filterApplicationIdResult.Response.Should().HaveCount(6);
            filterPermissionIdResult.Response.Should().HaveCount(1);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReq = new FilterRolePermissionLogicRequest { IncludeRelated = true };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
            
            foreach (var rolePermission in result.Response)
            {
                rolePermission.Active.Should().BeTrue();
                rolePermission.Permission.Should().NotBeNull();
                rolePermission.Permission.Active.Should().BeTrue();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReq = new FilterRolePermissionLogicRequest { IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(10);
            
            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().NotBeNull();
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReq = new FilterRolePermissionLogicRequest();

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(5);
            
            foreach (var rolePermission in result.Response)
            {
                rolePermission.Permission.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();

            var postReqInvalidCreatedBy = new FilterRolePermissionServiceRequest { CreatedBy = "asdfasdf" };
            var postReqInvalidCreatedOnDate = new FilterRolePermissionServiceRequest { CreatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidUpdatedBy = new FilterRolePermissionServiceRequest { UpdatedBy = "asdfasdf" };
            var postReqInvalidUpdatedOnDate = new FilterRolePermissionServiceRequest { UpdatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { -1 } };
            var postReqInvalidApplicationId = new FilterRolePermissionServiceRequest { ApplicationId = -1 };
            var postReqInvalidPermissionId = new FilterRolePermissionServiceRequest { PermissionId = -1 };
            
            // Act
            var invalidCreatedByResult = await _rolePermissionLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _rolePermissionLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _rolePermissionLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _rolePermissionLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidRolePermissionIdsResult = await _rolePermissionLogic.Filter(postReqInvalidRolePermissionIds);
            var invalidApplicationIdResult = await _rolePermissionLogic.Filter(postReqInvalidApplicationId);
            var invalidPermissionIdResult = await _rolePermissionLogic.Filter(postReqInvalidPermissionId);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidRolePermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidPermissionIdResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var rolePermission = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            var postReq = new FilterRolePermissionServiceRequest { IncludeReadOnly = true };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var roles =  await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 2);
            await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, roles[0].RoleId, permission.PermissionId);
            await _securityTestUtilities.RolePermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, roles[1].RoleId, permission.PermissionId);

            var postReq = new FilterRolePermissionServiceRequest { IncludeInactive = true, IncludeReadOnly = true };

            // Act
            var result = await _rolePermissionLogic.Filter(postReq);

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var roles =  await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 2);
            var testRecord = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, roles[0].RoleId, permission.PermissionId);
            
            var postReqInvalidCreatedBy = new FilterRolePermissionServiceRequest { CreatedBy = testRecord.CreatedBy };
            var postReqInvalidCreatedOnDate = new FilterRolePermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(testRecord.CreatedOn) };
            var postReqInvalidUpdatedBy = new FilterRolePermissionServiceRequest { UpdatedBy = testRecord.UpdatedBy };
            var postReqInvalidUpdatedOnDate = new FilterRolePermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)testRecord.UpdatedOn) };
            var postReqInvalidRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { testRecord.RolePermissionId } };
            var postReqInvalidApplicationId = new FilterRolePermissionServiceRequest { ApplicationId = testRecord.ApplicationId };
            var postReqInvalidRoleId = new FilterRolePermissionServiceRequest { RoleId = testRecord.RoleId };
            var postReqInvalidPermissionId = new FilterRolePermissionServiceRequest { PermissionId = testRecord.PermissionId };

            // Act
            var invalidCreatedByResult = await _rolePermissionLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _rolePermissionLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _rolePermissionLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _rolePermissionLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidRolePermissionIdsResult = await _rolePermissionLogic.Filter(postReqInvalidRolePermissionIds);
            var invalidApplicationIdResult = await _rolePermissionLogic.Filter(postReqInvalidApplicationId);
            var invalidRoleIdResult = await _rolePermissionLogic.Filter(postReqInvalidRoleId);
            var invalidPermissionIdResult = await _rolePermissionLogic.Filter(postReqInvalidPermissionId);

            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidRolePermissionIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidRoleIdResult.Response.Should().HaveCount(0);
            invalidPermissionIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateActiveTestRecords(1);
            var role = await _securityTestUtilities.Role.CreateActiveTestRecords(application[0].ApplicationId, 1);
            var permission =  await _securityTestUtilities.Permission.CreateActiveTestRecords(application[0].ApplicationId, 1);

            var insertReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application[0].ApplicationId,
                RoleId = role[0].RoleId,
                PermissionId = permission[0].PermissionId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _rolePermissionLogic.Insert(insertReq, _applicationLogic, _roleLogic, _permissionLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.ApplicationId.Should().Be(insertReq.ApplicationId);
            result.Response.RoleId.Should().Be(insertReq.RoleId);
            result.Response.PermissionId.Should().Be(insertReq.PermissionId);
            result.Response.Active.Should().BeTrue();
            result.Response.CreatedBy.Should().Be(TestConstants.CurrentUser);
            result.Response.UpdatedBy.Should().Be(TestConstants.CurrentUser);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var rolePermission = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();
            var recordToCreate = _securityTestUtilities.RolePermission.ConvertRolePermissionDtoToInsertUpdateRequest(rolePermission);

            var expectedUniqueError = _securityTestUtilities.RolePermission.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _rolePermissionLogic.Insert(recordToCreate, _applicationLogic, _roleLogic, _permissionLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var recordToCreate = new InsertUpdateRolePermissionRequest();

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _rolePermissionLogic.Insert(recordToCreate, _applicationLogic, _roleLogic, _permissionLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.RolePermission.CreateInsertUpdateRequestWithMaxLengthErrors(1, 1, 1);

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _rolePermissionLogic.Insert(recordToCreate, _applicationLogic, _roleLogic, _permissionLogic);

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();   

            var updateReq = new InsertUpdateRolePermissionRequest
            {
                Active = false,
                ApplicationId = recordToUpdate.ApplicationId,
                RoleId = recordToUpdate.RoleId,
                PermissionId = recordToUpdate.PermissionId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _rolePermissionLogic.Update(recordToUpdate.RolePermissionId, updateReq, _applicationLogic, _roleLogic, _permissionLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            result.Response.RoleId.Should().Be(updateReq.RoleId);
            result.Response.PermissionId.Should().Be(updateReq.PermissionId);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();   
            var recordToCopy = arrangeTestDataResponse.ActiveRolePermissions.Skip(1).FirstOrDefault();

            var updateReq = _securityTestUtilities.RolePermission.ConvertRolePermissionDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.ApplicationId = recordToCopy.ApplicationId;
            updateReq.RoleId = recordToCopy.RoleId;
            updateReq.PermissionId = recordToCopy.PermissionId;

            // Act
            var updateResult = await _rolePermissionLogic.Update(recordToUpdate.RolePermissionId, updateReq, _applicationLogic, _roleLogic, _permissionLogic);

            //Assert
            var expectedUniqueApplicationuserPermissionError = _securityTestUtilities.RolePermission.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueApplicationuserPermissionError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _rolePermissionLogic.Update(recordToUpdate.RolePermissionId, new InsertUpdateRolePermissionRequest(), _applicationLogic, _roleLogic, _permissionLogic);

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var recordToUpdate = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            var updateReq = _securityTestUtilities.RolePermission.ConvertRolePermissionDtoToInsertUpdateRequest(recordToUpdate);
            
            // Act
            var updateResult = await _rolePermissionLogic.Update(recordToUpdate.RolePermissionId, updateReq, _applicationLogic, _roleLogic, _permissionLogic);

            //Assert
            var expectedReadOnlyError = _securityTestUtilities.RolePermission.GetExpectedReadOnlyErrors();

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
            var arrangeTestDataResponse = await ArrangeRolePermissionTestData();
            var recordToDelete = arrangeTestDataResponse.ActiveRolePermissions.FirstOrDefault();   

            // Act
            var result = await _rolePermissionLogic.Delete(recordToDelete.RolePermissionId, TestConstants.CurrentUser);
            var getResult = await _rolePermissionLogic.GetById(recordToDelete.RolePermissionId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _rolePermissionLogic.Delete(-1, TestConstants.CurrentUser);

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
            var permission = (await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var role =  (await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedReadOnlyErrors();

            // Act
            var result = await _rolePermissionLogic.Delete(testRecord.RolePermissionId, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
