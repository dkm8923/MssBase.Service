using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;
using Shared.Logic.Common;
using Data.Security.Models;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserLogicTests : SecurityTestBase,
                                             IDefaultLogicTestsGetAll,
                                             IDefaultLogicTestsGetAllIncludeRelated,
                                             IDefaultLogicTestsGetById,
                                             IDefaultLogicTestsGetByIdIncludeRelated,
                                             IDefaultLogicTestsFilter,
                                             IDefaultLogicTestsFilterIncludeRelated,   
                                             IDefaultLogicTestsInsert, 
                                             IDefaultLogicTestsUpdate,
                                             IDefaultLogicTestsDelete
    {
        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            
            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            
            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(0);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true });

            // Assert
            result.Response.Should().HaveCount(1);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(2);

            foreach (var applicationUser in result.Response)
            {
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: true);
            }
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

            // Act
            var result = await _applicationUserLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(1);

            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserPermissions.Should().BeNull();
                applicationUser.ApplicationUserRoles.Should().BeNull();
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

            // Assert
            result.Response.Should().BeNull();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var testRecord = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet { IncludeRelated = true, IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.Active.Should().BeFalse();

            _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(result.Response, includeInactive: true); 
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Related_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();  

            // Act
            var result = await _applicationUserLogic.GetById(testRecord.ApplicationUserId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
            result.Response.ApplicationUserPermissions.Should().BeNull();
            result.Response.ApplicationUserRoles.Should().BeNull();
        }

        [Fact]
        public async Task PasswordChangeHistory_GetByApplicationUserId_Should_Return_Record()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            var pswdChangeHistoryResponse = await ArrangeApplicationUserPasswordChangeHistoryTestData(testRecord.ApplicationUserId);

            // Act
            var result = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testRecord.ApplicationUserId);

            // Assert
            result.Errors.Count.Should().Be(0);
            result.Response.Should().NotBeNull();
            result.Response.Should().HaveCount(1);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetByApplicationUserId_Should_Not_Return_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var invalidApplicationUserId = -1;

            // Act
            var result = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(invalidApplicationUserId);

            // Assert
            result.Errors.Count.Should().Be(0);
            result.Response.Should().HaveCount(0);
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
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);

            //create test roles for filtering tests
            var testApplicationUser1 = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                Email = "testEmail1@test.com",
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                DateOfBirth = new DateTime(1990, 1, 1),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic);

            var testApplicationUser2 = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 2, 2),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert
            }, _applicationLogic);

            await _applicationUserLogic.Update(testApplicationUser2.Response.ApplicationUserId, new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                Email = "testEmail2@test.com",
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                DateOfBirth = new DateTime(1991, 3, 2),
                Active = true,
                CurrentUser = TestConstants.SpecificCurrentUserForUpdate
            }, _applicationLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationUserLogicRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
            var postReqFilterCreatedOnDate = new FilterApplicationUserLogicRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationUserLogicRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
            var postReqFilterUpdatedOnDate = new FilterApplicationUserLogicRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationUserIds = new FilterApplicationUserLogicRequest { ApplicationUserIds = new List<int> { applicationUsers[0].ApplicationUserId, applicationUsers[1].ApplicationUserId, applicationUsers[2].ApplicationUserId } };
            var postReqFilterEmail = new FilterApplicationUserLogicRequest { Email = testApplicationUser1.Response.Email };
            var postReqFilterFirstName = new FilterApplicationUserLogicRequest { FirstName = testApplicationUser1.Response.FirstName };
            var postReqFilterLastName = new FilterApplicationUserLogicRequest { LastName = testApplicationUser1.Response.LastName };
            var postReqFilterDateOfBirth = new FilterApplicationUserLogicRequest { DateOfBirth = testApplicationUser1.Response.DateOfBirth };
            var postReqFilterApplicationId = new FilterApplicationUserLogicRequest { ApplicationId = application.ApplicationId };
            
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
            filterCreatedOnDateResult.Response.Should().HaveCount(7);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(7);
            filterApplicationUserIdsResult.Response.Should().HaveCount(3);
            filterEmailResult.Response.Should().HaveCount(1);
            filterFirstNameResult.Response.Should().HaveCount(1);
            filterLastNameResult.Response.Should().HaveCount(1);
            filterDateOfBirthResult.Response.Should().HaveCount(1);
            filterApplicationIdResult.Response.Should().HaveCount(7);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Related_Active_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

            var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(1);
            
            foreach (var applicationUser in result.Response)
            {
                applicationUser.Active.Should().BeTrue();
                
                _securityTestUtilities.ApplicationUser.VerifyIncludeRelatedDataOnApplicationUser(applicationUser, includeInactive: false); 
            }
        }
        
        [Fact]
        public async Task Default_Filter_Should_Return_Related_Inactive_Data()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();

            var postReq = new FilterApplicationUserLogicRequest { IncludeRelated = true, IncludeInactive = true };

            // Act
            var result = await _applicationUserLogic.Filter(postReq);

            // Assert
            result.Errors.Should().HaveCount(0);
            result.Response.Should().HaveCount(2);
            
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
            result.Response.Should().HaveCount(5);
            
            foreach (var applicationUser in result.Response)
            {
                applicationUser.ApplicationUserPermissions.Should().BeNull();
                applicationUser.ApplicationUserRoles.Should().BeNull();
            }
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Create_Record()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);

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
            await ClearAllSecurityTestTableData();
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
            await ClearAllSecurityTestTableData();
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                Active = false,
                ApplicationId = application.ApplicationId,
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            var dupeEmail = arrangeTestDataResponse.ActiveApplicationUsers.LastOrDefault().Email;

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();

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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var recordToUpdate = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();

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
    
        #region Reset Password

        [Fact]
        public async Task ApplicationUser_ResetPassword_Should_Reset_Password()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            //change password to ensure PasswordResetRequired is false before reset password test
            await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = TestConstants.DefaultTestUserPassword,
                CurrentUser = TestConstants.CurrentUser 
            });

            var testUserAfterPasswordChange = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());
            
            // Act
            var resetPasswordResult = await _applicationUserLogic.ResetPassword(testUser.Response.ApplicationUserId);
            var testUserAfterPasswordReset = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

            var passwordChangeHistoryAfterPasswordReset = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testUser.Response.ApplicationUserId); 

            // Assert
            testUserAfterPasswordChange.Response.PasswordResetRequired.Should().BeFalse();
            testUserAfterPasswordChange.Response.LastPasswordChangeDate.Should().NotBeNull();
            testUserAfterPasswordChange.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            resetPasswordResult.Errors.Should().BeNullOrEmpty();
            resetPasswordResult.Response.Should().NotBeNull();
            resetPasswordResult.Response.NewPassword.Should().NotBeNullOrEmpty();
            resetPasswordResult.Response.NewPassword.Should().NotBeEquivalentTo(testUser.Response.Password);
            
            testUserAfterPasswordReset.Response.PasswordResetRequired.Should().BeTrue();
            testUserAfterPasswordReset.Response.LastPasswordChangeDate.Should().NotBeNull();
            testUserAfterPasswordReset.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            passwordChangeHistoryAfterPasswordReset.Errors.Should().BeNullOrEmpty();
            passwordChangeHistoryAfterPasswordReset.Response.Should().HaveCount(2);
        }
            
        [Fact]
        public async Task ApplicationUser_ResetPassword_Should_Not_Reset_Password_Invalid_ApplicationUserId()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();

            // Act
            var resetPasswordResult = await _applicationUserLogic.ResetPassword(-1);
            
            // Assert
            resetPasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, resetPasswordResult.Errors);     
        }

        #endregion

        #region Change Password

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Change_Password()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = TestConstants.DefaultTestUserPassword,
                CurrentUser = TestConstants.CurrentUser 
            });
            
            var testUserAfterChangePassword = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet());

            var passwordChangeHistory = await _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(testUser.Response.ApplicationUserId); 

            // Assert
            changePasswordResult.Errors.Should().BeNullOrEmpty();

            testUserAfterChangePassword.Response.PasswordResetRequired.Should().BeFalse();
            testUserAfterChangePassword.Response.LastPasswordChangeDate.Should().NotBeNull();
            testUserAfterChangePassword.Response.LastPasswordChangeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            passwordChangeHistory.Errors.Should().BeNullOrEmpty();
            passwordChangeHistory.Response.Should().HaveCount(1);
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordRequiredFieldErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest());
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Invalid_ApplicationUserId()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedRecordDoesNotExistErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = 999, 
                NewPassword = TestConstants.DefaultTestUserPassword,
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Invalid_Password()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);
            var acceptablePassword = "!0TestPassword10!";
            
            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordInvalidPasswordErrors();

            await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = acceptablePassword,
                CurrentUser = TestConstants.CurrentUser 
            });

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = acceptablePassword, //setting new password to current password which should not be allowed    
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordMinMaxLengthErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(129, true),
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Min_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordMinMaxLengthErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = "!aB1", //special char, upper / lower case, and number
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_UpperCase_Letter_Required_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordUpperCaseRequiredErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = "!0testpassword0!", 
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_LowerCase_Letter_Required_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordLowerCaseRequiredErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                NewPassword = "!0TESTPASSWORD0!", 
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Special_Character_Required_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordSpecialCharacterRequiredErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                //NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(129, true),
                NewPassword = "TestPassword1", 
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        [Fact]
        public async Task ApplicationUser_ChangePassword_Should_Not_Change_Password_Number_Required_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var recordToCreate = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId, true);
            var testUser = await _applicationUserLogic.Insert(recordToCreate, _applicationLogic);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUser.GetExpectedChangePasswordNumberRequiredErrors();

            // Act
            var changePasswordResult = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { 
                ApplicationUserId = testUser.Response.ApplicationUserId, 
                //NewPassword = CommonUtilities.GenerateRandomAlphaNumericString(129, true),
                NewPassword = "TestPassword!", 
                CurrentUser = TestConstants.CurrentUser 
            });
            
            // Assert
            changePasswordResult.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, changePasswordResult.Errors);   
        }

        #endregion

    }
}
