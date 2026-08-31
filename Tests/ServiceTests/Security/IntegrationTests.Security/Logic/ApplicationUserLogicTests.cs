using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser.Service;
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
    public class ApplicationUserLogicTests : SecurityTestBase,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var activeResult = await _applicationUserLogic.GetAll(new BaseLogicGet());
            var inactiveResult = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            activeResult.Response.Should().HaveCount(0);
            inactiveResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());
            
            // Assert
            result.Response.Should().HaveCountGreaterThan(0);

            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserPermissions.Should().BeNull();
                applicationUser.ApplicationUserRoles.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, user.UserId, 1)).FirstOrDefault();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true });

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
            var users =  await _securityTestUtilities.User.CreateActiveTestRecords(2);
            
            await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, users[0].UserId);
            await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecord(application.ApplicationId, users[1].UserId);

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true, IncludeInactive = true });

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
            var users =  await _securityTestUtilities.User.CreateActiveTestRecords(3);
            
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, users[0].UserId, 1);
            await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, users[1].UserId, 1);
            await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, users[2].UserId);
            
            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());
            var resultWithIncludeInactiveFalse = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = false });

            // Assert
            result.Response.Should().BeNull();
            resultWithIncludeInactiveFalse.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeTrue();
            
            _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: false); 
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true, IncludeRelated = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();

            _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: false); 
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.ApplicationUserPermissions.Should().BeNull();
            result.Response.ApplicationUserRoles.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, user.UserId);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecord(application.ApplicationId, user.UserId);
    
            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeInactive = true, IncludeReadOnly = true });

            // Assert
            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(result.Response, testRecord);
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, user.UserId);

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

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
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = (await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newUser = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            
            var updateReq = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.UserId = newUser.UserId;

            // Act
            var updateResult = await _applicationUserLogic.Update(testRecord.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);
            var auditLogResult = await _applicationUserLogic.GetAuditLogsByApplicationUserId(testRecord.ApplicationUserId);

            // Assert
            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeUpdate);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUser);
            res.ReferenceId.Should().Be(testRecord.ApplicationUserId);

            var changeLog = ((JsonElement)res.ChangeLogJson).Deserialize<ApplicationUserChangeLog>();
            changeLog.Should().NotBeNull();
            changeLog.ApplicationId.Should().Be(updateReq.ApplicationId);
            changeLog.UserId.Should().Be(updateReq.UserId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<ApplicationUserDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.ApplicationUserId = res.ReferenceId;

            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = (await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId));
            
            // Act
            await _applicationUserLogic.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);
            var getResult = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());
            var auditLogResult = await _applicationUserLogic.GetAuditLogsByApplicationUserId(testRecord.ApplicationUserId);

            // Assert
            getResult.Response.Should().BeNull();

            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeDelete);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUser);
            res.ReferenceId.Should().Be(testRecord.ApplicationUserId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<ApplicationUserDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.ApplicationUserId = res.ReferenceId;

            _securityTestUtilities.ApplicationUser.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var testRecord = (await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId));
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newUser = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();

            var updateReq = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(testRecord);
            updateReq.ApplicationId = newApplication.ApplicationId;
            updateReq.UserId = newUser.UserId;

            // Act
            var updateResult = await _applicationUserLogic.Update(testRecord.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);
            await _applicationUserLogic.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);
            var auditLogResult = await _applicationUserLogic.GetAuditLogsByApplicationUserId(testRecord.ApplicationUserId);

            // Assert
            auditLogResult.Response.Should().HaveCount(2);

            var updateRes = auditLogResult.Response.First();
            updateRes.LogType.Should().Be(TestConstants.LogTypeUpdate);
            updateRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUser);
            updateRes.ReferenceId.Should().Be(testRecord.ApplicationUserId);

            var deleteRes = auditLogResult.Response.Last();
            deleteRes.LogType.Should().Be(TestConstants.LogTypeDelete);
            deleteRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeApplicationUser);
            deleteRes.ReferenceId.Should().Be(testRecord.ApplicationUserId);
        }

        class ApplicationUserChangeLog
        {
            public int? ApplicationId { get; set; }
            public int? ApplicationUserId { get; set; }
            public int? UserId { get; set; }
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

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
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var applicationUser = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            var applicationId = applicationUser.ApplicationId;
            var applicationUserId = applicationUser.ApplicationUserId;
            var userId = applicationUser.UserId;
            
            //create new user
            var newUser1 = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            //create new application user user with specific created / updated by values
            var testApplicationUser1Res = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest
            {
                ApplicationId = applicationId,
                UserId = newUser1.UserId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic, _applicationUserLogic, _userLogic);

            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newUser2 = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            await _applicationUserLogic.Update(testApplicationUser1Res.Response.ApplicationUserId, new InsertUpdateApplicationUserRequest
            {
                ApplicationId = newApplication.ApplicationId,
                UserId = newUser2.UserId,
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            }, _applicationLogic, _applicationUserLogic, _userLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationUserServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterApplicationUserServiceRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationUserServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
            var postReqFilterUpdatedOnDate = new FilterApplicationUserServiceRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationUserIds = new FilterApplicationUserServiceRequest { ApplicationUserIds = arrangeTestDataResponse.ActiveApplicationUsers.Select(x => x.ApplicationUserId).ToList() };
            var postReqFilterApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = applicationId };
            var postReqFilterUserId = new FilterApplicationUserServiceRequest { UserId = userId };
            
            // Act
            var filterCreatedByResult = await _applicationUserLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _applicationUserLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _applicationUserLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationUserLogic.Filter(postReqFilterUpdatedOnDate);
            var filterApplicationUserIdsResult = await _applicationUserLogic.Filter(postReqFilterApplicationUserIds);
            var filterApplicationIdResult = await _applicationUserLogic.Filter(postReqFilterApplicationId);
            var filterUserIdResult = await _applicationUserLogic.Filter(postReqFilterUserId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(1);
            filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterApplicationUserIdsResult.Response.Should().HaveCountGreaterThan(0);
            filterApplicationIdResult.Response.Should().HaveCountGreaterThan(0);
            filterUserIdResult.Response.Should().HaveCount(1);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser); 
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true); 
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            var postReq = new FilterApplicationUserLogicRequest();

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCountGreaterThan(0);
            
            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserRoles.Should().BeNull();
                applicationUser.ApplicationUserPermissions.Should().BeNull();
            }
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

            var postReqInvalidCreatedBy = new FilterApplicationUserServiceRequest { CreatedBy = "asdfasdf" };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserServiceRequest { CreatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidUpdatedBy = new FilterApplicationUserServiceRequest { UpdatedBy = "asdfasdf" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserServiceRequest { UpdatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidApplicationUserIds = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { -1 } };
            var postReqInvalidApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = -1 };
            var postReqInvalidUserId = new FilterApplicationUserServiceRequest { UserId = -1 };
            
            // Act
            var invalidCreatedByResult = await _applicationUserLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationUserLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationUserLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationUserLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidApplicationUserIdsResult = await _applicationUserLogic.Filter(postReqInvalidApplicationUserIds);
            var invalidApplicationIdResult = await _applicationUserLogic.Filter(postReqInvalidApplicationId);
            var invalidUserIdResult = await _applicationUserLogic.Filter(postReqInvalidUserId);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidUserIdResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords()).FirstOrDefault();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, user.UserId);

            var postReq = new FilterApplicationUserServiceRequest { IncludeReadOnly = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

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
            var users =  await _securityTestUtilities.User.CreateActiveTestRecords(2);
            await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, users[0].UserId);
            await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecord(application.ApplicationId, users[1].UserId);

            var postReq = new FilterApplicationUserServiceRequest { IncludeInactive = true, IncludeReadOnly = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

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
            var users =  await _securityTestUtilities.User.CreateActiveTestRecords(2);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, users[0].UserId);
            
            var postReqInvalidCreatedBy = new FilterApplicationUserLogicRequest { CreatedBy = testRecord.CreatedBy };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserLogicRequest { CreatedOnDate = DateOnly.FromDateTime(testRecord.CreatedOn) };
            var postReqInvalidUpdatedBy = new FilterApplicationUserLogicRequest { UpdatedBy = testRecord.UpdatedBy };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserLogicRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)testRecord.UpdatedOn) };
            var postReqInvalidApplicationUserIds = new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { testRecord.ApplicationUserId } };
            var postReqInvalidApplicationId = new FilterApplicationUserLogicRequest { ApplicationId = testRecord.ApplicationId };
            var postReqInvalidApplicationUserId = new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { testRecord.ApplicationUserId } };
            var postReqInvalidUserId = new FilterApplicationUserLogicRequest { UserId = testRecord.UserId };

            // Act
            var invalidCreatedByResult = await _applicationUserLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationUserLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationUserLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationUserLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidApplicationUserIdsResult = await _applicationUserLogic.Filter(postReqInvalidApplicationUserIds);
            var invalidApplicationIdResult = await _applicationUserLogic.Filter(postReqInvalidApplicationId);
            var invalidApplicationUserIdResult = await _applicationUserLogic.Filter(postReqInvalidApplicationUserId);
            var invalidUserIdResult = await _applicationUserLogic.Filter(postReqInvalidUserId);

            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidApplicationUserIdResult.Response.Should().HaveCount(0);
            invalidUserIdResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        //securityTestData

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = (await _securityTestUtilities.Application.CreateActiveTestRecords(1)).FirstOrDefault();
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            
            var insertReq = new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                UserId = user.UserId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _userLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.ApplicationId.Should().Be(insertReq.ApplicationId);
            result.Response.UserId.Should().Be(insertReq.UserId);
            result.Response.Active.Should().BeTrue();
            result.Response.CreatedBy.Should().Be(TestConstants.CurrentUser);
            result.Response.UpdatedBy.Should().Be(TestConstants.CurrentUser);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var applicationUser = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            var recordToCreate = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(applicationUser);

            var expectedUniqueError = _securityTestUtilities.ApplicationUser.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _userLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var recordToCreate = new InsertUpdateApplicationUserRequest();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _userLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithMaxLengthErrors(1, 1);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _userLogic);

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();   

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Active = false,
                ApplicationId = recordToUpdate.ApplicationId,
                UserId = recordToUpdate.UserId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            result.Response.UserId.Should().Be(updateReq.UserId);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();   
            
            var activeUser = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).FirstOrDefault();
            var recordToCopy = (await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(arrangeTestDataResponse.ActiveApplications[0].ApplicationId, activeUser.UserId, 1)).FirstOrDefault();
            
            var updateReq = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.ApplicationId = recordToCopy.ApplicationId;
            updateReq.UserId = recordToCopy.UserId;

            // Act
            var updateResult = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);

            //Assert
            var expectedUniqueApplicationuserPermissionError = _securityTestUtilities.ApplicationUser.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueApplicationuserPermissionError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, new InsertUpdateApplicationUserRequest(), _applicationLogic, _applicationUserLogic, _userLogic);

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
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords()).FirstOrDefault();
            var recordToUpdate = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, user.UserId);

            var updateReq = _securityTestUtilities.ApplicationUser.ConvertApplicationUserDtoToInsertUpdateRequest(recordToUpdate);
            
            // Act
            var updateResult = await _applicationUserLogic.Update(recordToUpdate.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);

            //Assert
            var expectedReadOnlyError = _securityTestUtilities.ApplicationUser.GetExpectedReadOnlyErrors();

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToDelete = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();   

            // Act
            var result = await _applicationUserLogic.Delete(recordToDelete.ApplicationUserId, TestConstants.CurrentUser);
            var getResult = await _applicationUserLogic.GetById(recordToDelete.ApplicationUserId, new BaseLogicGet { IncludeInactive = true });

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
            var result = await _applicationUserLogic.Delete(-1, TestConstants.CurrentUser);

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
            var user =  (await _securityTestUtilities.User.CreateActiveTestRecords()).FirstOrDefault();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(application.ApplicationId, user.UserId);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedReadOnlyErrors();

            // Act
            var result = await _applicationUserLogic.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
