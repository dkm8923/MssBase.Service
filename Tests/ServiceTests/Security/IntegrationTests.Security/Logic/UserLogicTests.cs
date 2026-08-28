using Dto.Security.User;
using Dto.Security.User.Logic;
using Dto.Security.User.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;
using Shared.Logic.Common;
using Data.Security.Models;
using System.Text.Json;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class UserLogicTests : SecurityTestBase,
                                  IDefaultLogicTestsGetAll,
                                //   IDefaultLogicTestsGetAllIncludeRelated,
                                  IDefaultLogicTestsGetAllReadOnly,
                                  IDefaultLogicTestsGetById,
                                //   IDefaultLogicTestsGetByIdIncludeRelated,
                                  IDefaultLogicTestsGetByIdReadOnly,
                                  IDefaultLogicTestsGetAuditLogsById,
                                  IDefaultLogicTestsFilter,
                                //   IDefaultLogicTestsFilterIncludeRelated,  
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        // [Fact]
        // public async Task Default_GetAll_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

        //     // Act
        //     var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true });

        //     // Assert
        //     result.Response.Should().HaveCount(1);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
        //     }
        // }

        // [Fact]
        // public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

        //     // Act
        //     var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

        //     // Assert
        //     result.Response.Should().HaveCount(2);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
        //     }
        // }

        // [Fact]
        // public async Task Default_GetAll_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

        //     // Act
        //     var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

        //     // Assert
        //     result.Response.Should().HaveCount(1);

        //     foreach (var applicationUser in result.Response)
        //     {
        //         applicationUser.ApplicationUserPermissions.Should().BeNull();
        //         applicationUser.ApplicationUserRoles.Should().BeNull();
        //     }
        // }

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
            await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);

            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true });

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
            await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
            await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);

            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet { IncludeReadOnly = true, IncludeInactive = true });

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
            await _securityTestUtilities.User.CreateActiveTestRecords(1);
            await _securityTestUtilities.User.CreateInactiveTestRecords(1);
            await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);

            // Act
            var result = await _userLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();

            // Act
            var result = await _userLogic.GetById(testRecord.UserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testRecord = arrangeTestDataResponse.InactiveUsers.FirstOrDefault();

            // Act
            var result = await _userLogic.GetById(testRecord.UserId, new BaseLogicGet());

            // Assert
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testRecord = arrangeTestDataResponse.InactiveUsers.FirstOrDefault();

            // Act
            var result = await _userLogic.GetById(testRecord.UserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        // [Fact]
        // public async Task Default_GetById_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
        //     var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

        //     // Act
        //     var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeRelated = true });

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.Active.Should().BeTrue();

        //      _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: false); 
        // }

        // [Fact]
        // public async Task Default_GetById_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
        //     var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();  

        //     // Act
        //     var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.Active.Should().BeFalse();

        //     _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: true); 
        // }

        // [Fact]
        // public async Task Default_GetById_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
        //     var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

        //     // Act
        //     var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

        //     // Assert
        //     result.Response.Should().NotBeNull();
        //     result.Response.ApplicationUserPermissions.Should().BeNull();
        //     result.Response.ApplicationUserRoles.Should().BeNull();
        // }

        // [Fact]
        // public async Task PasswordChangeHistory_GetByApplicationUserId_Should_Return_Record()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
        //     var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
        //     var pswdChangeHistoryResponse = await ArrangeApplicationUserPasswordChangeHistoryTestData(testRecord.ApplicationUserId);

        //     // Act
        //     var result = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testRecord.ApplicationUserId);

        //     // Assert
        //     result.Errors.Count.Should().Be(0);
        //     result.Response.Should().NotBeNull();
        //     result.Response.Should().HaveCount(1);
        // }

        // [Fact]
        // public async Task PasswordChangeHistory_GetByApplicationUserId_Should_Not_Return_Record_Invalid_Id()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var invalidApplicationUserId = -1;

        //     // Act
        //     var result = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(invalidApplicationUserId);

        //     // Assert
        //     result.Errors.Count.Should().Be(0);
        //     result.Response.Should().HaveCount(0);
        // }

        [Fact]
        public async Task Default_GetById_Should_Return_Active_ReadOnly_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];
            
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
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];
    
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
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var res = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
            var testRecord = res[0];

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
            var testRecord = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).First();
            
            var updateReq = _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(testRecord);
            updateReq.Email = "UpdatedEmail@Test.com";
            updateReq.FirstName = "Updated FirstName";
            updateReq.LastName = "Updated LastName";
            updateReq.DateOfBirth = new DateTime(2000, 1, 1);

            // Act
            var updateResult = await _userLogic.Update(testRecord.UserId, updateReq);
            var auditLogResult = await _userLogic.GetAuditLogsByUserId(testRecord.UserId);

            // Assert
            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeUpdate);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeUser);
            res.ReferenceId.Should().Be(testRecord.UserId);

            var changeLog = ((JsonElement)res.ChangeLogJson).Deserialize<UserChangeLog>();
            changeLog.Should().NotBeNull();
            changeLog.Email.Should().Be(updateReq.Email);
            changeLog.FirstName.Should().Be(updateReq.FirstName);
            changeLog.LastName.Should().Be(updateReq.LastName);
            changeLog.DateOfBirth.Should().Be(updateReq.DateOfBirth);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<UserDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.UserId = res.ReferenceId;

            _securityTestUtilities.User.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).First();

            // Act
            await _userLogic.Delete(testRecord.UserId, TestConstants.CurrentUser);
            var getResult = await _userLogic.GetById(testRecord.UserId, new BaseLogicGet());
            var auditLogResult = await _userLogic.GetAuditLogsByUserId(testRecord.UserId);

            // Assert
            getResult.Response.Should().BeNull();

            auditLogResult.Response.Should().HaveCount(1);

            var res = auditLogResult.Response.First();
            res.LogType.Should().Be(TestConstants.LogTypeDelete);
            res.ReferenceType.Should().Be(TestConstants.ReferenceTypeUser);
            res.ReferenceId.Should().Be(testRecord.UserId);

            var recordStateBeforeChange = ((JsonElement)res.RecordStateBeforeChangeJson).Deserialize<UserDto>();
            recordStateBeforeChange.Should().NotBeNull();
            recordStateBeforeChange.UserId = res.ReferenceId;

            _securityTestUtilities.User.VerifyTestRecordValuesMatch(recordStateBeforeChange, testRecord);
        }

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Return_Update_And_Delete_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = (await _securityTestUtilities.User.CreateActiveTestRecords(1)).First();

            var updateReq = _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(testRecord);
            updateReq.Email = "UpdatedEmail@Test.com";
            updateReq.FirstName = "Updated FirstName";
            updateReq.LastName = "Updated LastName";
            updateReq.DateOfBirth = new DateTime(2000, 1, 1);

            // Act
            var updateResult = await _userLogic.Update(testRecord.UserId, updateReq);
            await _userLogic.Delete(testRecord.UserId, TestConstants.CurrentUser);
            var auditLogResult = await _userLogic.GetAuditLogsByUserId(testRecord.UserId);

            // Assert
            auditLogResult.Response.Should().HaveCount(2);

            var updateRes = auditLogResult.Response.First();
            updateRes.LogType.Should().Be(TestConstants.LogTypeUpdate);
            updateRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeUser);
            updateRes.ReferenceId.Should().Be(testRecord.UserId);

            var deleteRes = auditLogResult.Response.Last();
            deleteRes.LogType.Should().Be(TestConstants.LogTypeDelete);
            deleteRes.ReferenceType.Should().Be(TestConstants.ReferenceTypeUser);
            deleteRes.ReferenceId.Should().Be(testRecord.UserId);
        }

        class UserChangeLog
        {
            public string? Email { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var postReq = new FilterUserLogicRequest { };

            // Act
            var result = await _userLogic.Filter(postReq);

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
            var arrangeTestDataResponse = await ArrangeUserTestData();

            var postReq = new FilterUserLogicRequest { IncludeInactive = true };

            // Act
            var result = await _userLogic.Filter(postReq);

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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            
            var postReqInvalidCreatedBy = new FilterUserLogicRequest { CreatedBy = "InvalidCreatedBy" };
            var postReqInvalidCreatedOnDate = new FilterUserLogicRequest { CreatedOnDate = DateOnly.FromDateTime(new DateTime(1900, 1, 1)) };
            var postReqInvalidUpdatedBy = new FilterUserLogicRequest { UpdatedBy = "InvalidUpdatedBy" };
            var postReqInvalidUpdatedOnDate = new FilterUserLogicRequest { UpdatedOnDate = DateOnly.FromDateTime(new DateTime(1900, 1, 1)) };
            var postReqInvalidEmail = new FilterUserLogicRequest { Email = "invalid@test.com" };
            var postReqInvalidFirstName = new FilterUserLogicRequest { FirstName = "InvalidFirstName" };
            var postReqInvalidLastName = new FilterUserLogicRequest { LastName = "InvalidLastName" };
            var postReqInvalidDateOfBirth = new FilterUserLogicRequest { DateOfBirth = new DateTime(1900, 1, 1) };
            
            // Act
            var invalidCreatedByResult = await _userLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _userLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _userLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _userLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidEmailResult = await _userLogic.Filter(postReqInvalidEmail);
            var invalidFirstNameResult = await _userLogic.Filter(postReqInvalidFirstName);
            var invalidLastNameResult = await _userLogic.Filter(postReqInvalidLastName);
            var invalidDateOfBirthResult = await _userLogic.Filter(postReqInvalidDateOfBirth);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidEmailResult.Response.Should().HaveCount(0);
            invalidFirstNameResult.Response.Should().HaveCount(0);
            invalidLastNameResult.Response.Should().HaveCount(0);
            invalidDateOfBirthResult.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_Filter_Should_Filter_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var users = await _securityTestUtilities.User.CreateActiveTestRecords();

            //create test roles for filtering tests
            var testUser1 = await _userLogic.Insert(new InsertUpdateUserRequest
            {
                Email = "testEmail1@test.com",
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                DateOfBirth = new DateTime(1990, 1, 1),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            });

            var testUser2 = await _userLogic.Insert(new InsertUpdateUserRequest
            {
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 2, 2),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            });

            await _userLogic.Update(testUser2.Response.UserId, new InsertUpdateUserRequest
            {
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 3, 2),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            });

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterUserLogicRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterUserLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterUserLogicRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
            var postReqFilterUpdatedOnDate = new FilterUserLogicRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterUserIds = new FilterUserLogicRequest { UserIds = new List<int> { users[0].UserId } };
            var postReqFilterEmail = new FilterUserLogicRequest { Email = testUser1.Response.Email };
            var postReqFilterFirstName = new FilterUserLogicRequest { FirstName = testUser1.Response.FirstName };
            var postReqFilterLastName = new FilterUserLogicRequest { LastName = testUser1.Response.LastName };
            var postReqFilterDateOfBirth = new FilterUserLogicRequest { DateOfBirth = testUser1.Response.DateOfBirth };
            
            // Act
            var filterCreatedByResult = await _userLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _userLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _userLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _userLogic.Filter(postReqFilterUpdatedOnDate);
            var filterUserIdsResult = await _userLogic.Filter(postReqFilterUserIds);
            var filterEmailResult = await _userLogic.Filter(postReqFilterEmail);
            var filterFirstNameResult = await _userLogic.Filter(postReqFilterFirstName);
            var filterLastNameResult = await _userLogic.Filter(postReqFilterLastName);
            var filterDateOfBirthResult = await _userLogic.Filter(postReqFilterDateOfBirth);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(2);
            filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);
            filterUserIdsResult.Response.Should().HaveCount(1);
            filterEmailResult.Response.Should().HaveCount(1);
            filterFirstNameResult.Response.Should().HaveCount(1);
            filterLastNameResult.Response.Should().HaveCount(1);
            filterDateOfBirthResult.Response.Should().HaveCount(1);
        }

        // [Fact]
        // public async Task Default_Filter_Should_Return_Related_Active_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

        //     var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true };

        //     // Act
        //     var result = await _applicationUserLogic.Filter(postReq);

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(1);
            
        //     foreach (var applicationUser in result.Response)
        //     {
        //         applicationUser.Active.Should().BeTrue();
                
        //         _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false); 
        //     }
        // }
        
        // [Fact]
        // public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

        //     var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true, IncludeInactive = true };

        //     // Act
        //     var result = await _applicationUserLogic.Filter(postReq);

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCount(2);
            
        //     foreach (var applicationUser in result.Response)
        //     {
        //         _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
        //     }
        // }
        
        // [Fact]
        // public async Task Default_Filter_Should_Not_Return_Related_Data()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestData();

        //     var postReq = new FilterApplicationUserLogicRequest();

        //     // Act
        //     var result = await _applicationUserLogic.Filter(postReq);

        //     // Assert
        //     result.Errors.Should().HaveCount(0);
        //     result.Response.Should().HaveCountGreaterThan(0);
            
        //     foreach (var applicationUser in result.Response)
        //     {
        //         applicationUser.ApplicationUserPermissions.Should().BeNull();
        //         applicationUser.ApplicationUserRoles.Should().BeNull();
        //     }
        // }

        [Fact]
        public async Task Default_Filter_Should_Return_Active_ReadOnly_Data()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
            await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);

           var postReq = new FilterUserLogicRequest { IncludeReadOnly = true };

            // Act
            var result = await _userLogic.Filter(postReq);

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

            await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
            await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);

            var postReq = new FilterUserLogicRequest { IncludeInactive = true, IncludeReadOnly = true };

            // Act
            var result = await _userLogic.Filter(postReq);

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

            var testRecord = (await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1)).First();
            await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);
            
            var postReqInvalidCreatedBy = new FilterUserLogicRequest { CreatedBy = testRecord.CreatedBy };
            var postReqInvalidCreatedOnDate = new FilterUserLogicRequest { CreatedOnDate = DateOnly.FromDateTime(testRecord.CreatedOn) };
            var postReqInvalidUpdatedBy = new FilterUserLogicRequest { UpdatedBy = testRecord.UpdatedBy };
            var postReqInvalidUpdatedOnDate = new FilterUserLogicRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)testRecord.UpdatedOn) };
            var postReqInvalidEmail = new FilterUserLogicRequest { Email = testRecord.Email };
            var postReqInvalidFirstName = new FilterUserLogicRequest { FirstName = testRecord.FirstName };
            var postReqInvalidLastName = new FilterUserLogicRequest { LastName = testRecord.LastName };
            var postReqInvalidDateofBirth = new FilterUserLogicRequest { DateOfBirth = testRecord.DateOfBirth };

            // Act
            var invalidCreatedByResult = await _userLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _userLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _userLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _userLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidEmailResult = await _userLogic.Filter(postReqInvalidEmail);
            var invalidFirstNameResult = await _userLogic.Filter(postReqInvalidFirstName);
            var invalidLastNameResult = await _userLogic.Filter(postReqInvalidLastName);
            var invalidDateofBirthResult = await _userLogic.Filter(postReqInvalidDateofBirth);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidEmailResult.Response.Should().HaveCount(0);
            invalidFirstNameResult.Response.Should().HaveCount(0);
            invalidLastNameResult.Response.Should().HaveCount(0);
            invalidDateofBirthResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var insertReq = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();

            // Act
            var result = await _userLogic.Insert(insertReq);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.Email.Should().Be(insertReq.Email);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            var recordToCreate = _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(testRecord);

            var expectedUniqueEmailError = _securityTestUtilities.User.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _userLogic.Insert(recordToCreate);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueEmailError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = new InsertUpdateUserRequest();

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _userLogic.Insert(recordToCreate);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithMaxLengthErrors();

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _userLogic.Insert(recordToCreate);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task User_Insert_Should_Not_Create_Record_Invalid_Email_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();
            recordToCreate.Email = "invalidEmail";

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedInvalidEmailFieldErrors();

            // Act
            var result = await _userLogic.Insert(recordToCreate);

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
            var testRecord = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            var updateReq = new InsertUpdateUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                CurrentUser = "IntegrationTest"
            };

            // Act
            var result = await _userLogic.Update(testRecord.UserId, updateReq);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Email.Should().Be(updateReq.Email);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            var existingRecord = (await _securityTestUtilities.User.CreateActiveTestRecords()).FirstOrDefault();
            
            var updateReq = _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.Email = existingRecord.Email;

            // Act
            var updateResult = await _userLogic.Update(recordToUpdate.UserId, updateReq);

            //Assert
            var expectedUniqueEmailError = _securityTestUtilities.ApplicationUser.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueEmailError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _userLogic.Update(recordToUpdate.UserId, new InsertUpdateUserRequest());

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task User_Update_Should_Not_Create_Record_Invalid_Email_Error()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            recordToUpdate.Email = "invalidEmail";

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedInvalidEmailFieldErrors();

            // Act
            var result = await _userLogic.Update(recordToUpdate.UserId, _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(recordToUpdate));

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();

            var updateReq = _securityTestUtilities.User.ConvertUserDtoToInsertUpdateRequest(recordToUpdate);
            
            // Act
            var updateResult = await _userLogic.Update(recordToUpdate.UserId, updateReq);

            //Assert
            var expectedReadOnlyError = _securityTestUtilities.User.GetExpectedReadOnlyErrors();

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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();

            // Act
            var result = await _userLogic.Delete(testRecord.UserId, TestConstants.CurrentUser);
            var getResult = await _userLogic.GetById(testRecord.UserId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _userLogic.Delete(-1, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        // [Fact]
        // public async Task ApplicationUser_Delete_Should_Not_Delete_Record_ApplicationUserPermission_Foreign_Key_Dependency_Exists()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
        //     var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
        //     var permission = _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(testRecord.ApplicationId).Result;
        //     await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(testRecord.ApplicationId, testRecord.ApplicationUserId, permission.PermissionId);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedApplicationUserPermissionForeignKeyErrors();

        //     // Act
        //     var result = await _applicationUserLogic.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);
            
        //     // Assert
        //     result.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);    
        // }

        // [Fact]
        // public async Task ApplicationUser_Delete_Should_Not_Delete_Record_ApplicationUserRole_Foreign_Key_Dependency_Exists()
        // {
        //     // Arrange
        //     var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
        //     var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
        //     var role = _securityTestUtilities.Role.CreateSingleRoleTestRecord(testRecord.ApplicationId).Result;
        //     await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(testRecord.ApplicationId, testRecord.ApplicationUserId, role.RoleId);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedApplicationUserRoleForeignKeyErrors();

        //     // Act
        //     var result = await _applicationUserLogic.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);
            
        //     // Assert
        //     result.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);    
        // }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_ReadOnly_Error()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = (await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1)).First();

            var expectedFieldErrors = _securityTestUtilities.User.GetExpectedReadOnlyErrors();

            // Act
            var result = await _userLogic.Delete(testRecord.UserId, TestConstants.CurrentUser);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    
        #region Reset Password

        // [Fact]
        // public async Task ApplicationUser_ResetPassword_Should_Reset_Password()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     //change password to ensure PasswordResetRequired is false before reset password test
        //     await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = TestConstants.DefaultTestUserPassword,
        //         CurrentUser = TestConstants.CurrentUser 
        //     });

        //     var testUserAfterPasswordChange = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());
            
        //     // Act
        //     var resetPasswordResult = await _applicationUserLogic.ResetPassword(testUser.Response.ApplicationUserId);
        //     var testUserAfterPasswordReset = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

        //     var passwordChangeHistoryAfterPasswordReset = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testUser.Response.ApplicationUserId); 

        //     // Assert
        //     testUserAfterPasswordChange.Response.PasswordResetRequired.Should().BeFalse();
        //     testUserAfterPasswordChange.Response.LastPasswordChangeDate.Should().NotBeNull();
        //     testUserAfterPasswordChange.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        //     resetPasswordResult.Errors.Should().BeNullOrEmpty();
        //     resetPasswordResult.Response.Should().NotBeNull();
        //     resetPasswordResult.Response.NewPassword.Should().NotBeNullOrEmpty();
        //     resetPasswordResult.Response.NewPassword.Should().NotBeEquivalentTo(testUser.Response.Password);
            
        //     testUserAfterPasswordReset.Response.PasswordResetRequired.Should().BeTrue();
        //     testUserAfterPasswordReset.Response.LastPasswordChangeDate.Should().NotBeNull();
        //     testUserAfterPasswordReset.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        //     passwordChangeHistoryAfterPasswordReset.Errors.Should().BeNullOrEmpty();
        //     passwordChangeHistoryAfterPasswordReset.Response.Should().HaveCount(2);
        // }
            
        // [Fact]
        // public async Task ApplicationUser_ResetPassword_Should_Not_Reset_Password_Invalid_ApplicationUserId()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();

        //     // Act
        //     var resetPasswordResult = await _applicationUserLogic.ResetPassword(-1);
            
        //     // Assert
        //     resetPasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, resetPasswordResult.Errors);     
        // }

        #endregion

        #region Change Password

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Change_Password()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = TestConstants.DefaultTestUserPassword,
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     var testUserAfterChangePassword = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

        //     var passwordChangeHistory = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testUser.Response.ApplicationUserId); 

        //     // Assert
        //     changePasswordResult.Errors.Should().BeNullOrEmpty();

        //     testUserAfterChangePassword.Response.PasswordResetRequired.Should().BeFalse();
        //     testUserAfterChangePassword.Response.LastPasswordChangeDate.Should().NotBeNull();
        //     testUserAfterChangePassword.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        //     passwordChangeHistory.Errors.Should().BeNullOrEmpty();
        //     passwordChangeHistory.Response.Should().HaveCount(1);
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Required_Field_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordRequiredFieldErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest());
            
        //     // Assert
        //     changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Invalid_ApplicationUserId()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = 999, 
        //         NewPassword = TestConstants.DefaultTestUserPassword,
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Invalid_Password()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
        //     var acceptablePassword = "!0TestPassword10!";
            
        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordInvalidPasswordErrors();

        //     await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = acceptablePassword,
        //         CurrentUser = TestConstants.CurrentUser 
        //     });

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = acceptablePassword, //setting new password to current password which should not be allowed    
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Max_Length_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordMinMaxLengthErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(_passwordValidationConfigMonitor.CurrentValue.MaxLength + 1, true),
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Min_Length_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordMinMaxLengthErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = "!aB1", //special char, upper / lower case, and number
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //     LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_UpperCase_Letter_Required_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordUpperCaseRequiredErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = "!0testpassword0!", 
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     if (_passwordValidationConfigMonitor.CurrentValue.RequireUppercase)
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //         LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors); 
        //     }
        //     else
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(0);
        //     }
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_LowerCase_Letter_Required_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordLowerCaseRequiredErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         NewPassword = "!0TESTPASSWORD0!", 
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     if (_passwordValidationConfigMonitor.CurrentValue.RequireLowercase)
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //         LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors); 
        //     }
        //     else
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(0);
        //     }  
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Special_Character_Required_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordSpecialCharacterRequiredErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         //NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(129, true),
        //         NewPassword = "TestPassword1", 
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //     if (_passwordValidationConfigMonitor.CurrentValue.RequireNonAlphanumeric)
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //         LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors); 
        //     }
        //     else
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(0);
        //     }     
        // }

        // [Fact]
        // public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Number_Required_Errors()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        //     var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
        //     var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

        //     var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordNumberRequiredErrors();

        //     // Act
        //     var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
        //         ApplicationUserId = testUser.Response.ApplicationUserId, 
        //         //NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(129, true),
        //         NewPassword = "TestPassword!", 
        //         CurrentUser = TestConstants.CurrentUser 
        //     });
            
        //     // Assert
        //    if (_passwordValidationConfigMonitor.CurrentValue.RequireDigit)
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

        //         LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors); 
        //     }
        //     else
        //     {
        //         changePasswordResult.Errors.Count.Should().Be(0);
        //     }      
        // }

        #endregion

    }
}
