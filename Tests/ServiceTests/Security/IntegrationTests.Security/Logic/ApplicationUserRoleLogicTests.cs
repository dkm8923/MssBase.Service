using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Dto.Security.ApplicationUserRole.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;
using Dto.Security.Role;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserRoleLogicTests : SecurityTestBase,
                                                       IDefaultLogicTestsGetAll,
                                                       IDefaultLogicTestsGetById,
                                                       IDefaultLogicTestsFilter,
                                                       IDefaultLogicTestsInsert, 
                                                       IDefaultLogicTestsUpdate,
                                                       IDefaultLogicTestsDelete
    {
        #region Private

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Should_Return_Active_Data()
        {
            // Arrange
            await ArrangeSecurityTestData();

            // Act
            var result = await _applicationUserRoleLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(125);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ArrangeSecurityTestData();

            // Act
            var result = await _applicationUserRoleLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(375);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            // Act
            var activeResult = await _applicationUserRoleLogic.GetAll(new BaseLogicGet());
            var inactiveResult = await _applicationUserRoleLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            activeResult.Response.Should().HaveCount(0);
            inactiveResult.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();  

            // Act
            var result = await _applicationUserRoleLogic.GetById(testRecord.ApplicationUserRoleId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveApplicationUserRoles.FirstOrDefault();  

            // Act
            var result = await _applicationUserRoleLogic.GetById(testRecord.ApplicationUserRoleId, new BaseLogicGet());
            var resultWithIncludeInactiveFalse = await _applicationUserRoleLogic.GetById(testRecord.ApplicationUserRoleId, new BaseLogicGet { IncludeInactive = false });

            // Assert
            result.Response.Should().BeNull();
            resultWithIncludeInactiveFalse.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Return_Inactive_Record()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveApplicationUserRoles.FirstOrDefault();  

            // Act
            var result = await _applicationUserRoleLogic.GetById(testRecord.ApplicationUserRoleId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Return_Active_Data()
        {
            // Arrange
            await ArrangeSecurityTestData();

            var postReq = new FilterApplicationUserRoleLogicRequest { };

            // Act
            var result = await _applicationUserRoleLogic.Filter(postReq);

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
            await ArrangeSecurityTestData();

            var postReq = new FilterApplicationUserRoleLogicRequest { IncludeInactive = true };

            // Act
            var result = await _applicationUserRoleLogic.Filter(postReq);

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
            var securityTestData = await ArrangeSecurityTestData();
            var applicationId = securityTestData.ActiveApplications.FirstOrDefault().ApplicationId;
            var applicationUserId = securityTestData.ActiveApplicationUsers.Where(x => x.ApplicationId == applicationId).FirstOrDefault().ApplicationUserId;
            var roleId = securityTestData.ActiveRoles.Where(x => x.ApplicationId == applicationId).FirstOrDefault().RoleId;
            var applicationUserRoleId = securityTestData.ActiveApplicationUserRoles.FirstOrDefault().ApplicationUserRoleId;

            //create new role
            var testRole1 = await _roleLogic.Insert(new InsertUpdateRoleRequest
            {
                ApplicationId = applicationId,
                Name = "Test Role Name 1",
                Description = "Test Role Description 1",
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            }, _applicationLogic);

            //create new application user role with specific created / updated by values
            var testApplicationUserRole1Res = await _applicationUserRoleLogic.Insert(new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = applicationId,
                ApplicationUserId = applicationUserId,
                RoleId = testRole1.Response.RoleId,
                Active = true,
                CurrentUser = "IntegrationTestInsert"
            }, _applicationLogic, _applicationUserLogic, _roleLogic);

            await _applicationUserRoleLogic.Update(testApplicationUserRole1Res.Response.ApplicationUserRoleId, new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = applicationId,
                ApplicationUserId = applicationUserId,
                RoleId = testRole1.Response.RoleId,
                Active = true,
                CurrentUser = "IntegrationTestUpdate"
            }, _applicationLogic, _applicationUserLogic, _roleLogic);

            var todaysUtcDate = LogicTestUtilities.GetTodaysUtcDateOnly();

            var postReqFilterCreatedBy = new FilterApplicationUserRoleServiceRequest { CreatedBy = "IntegrationTestInsert" };
            var postReqFilterCreatedOnDate = new FilterApplicationUserRoleServiceRequest { CreatedOnDate = todaysUtcDate };
            var postReqFilterUpdatedBy = new FilterApplicationUserRoleServiceRequest { UpdatedBy = "IntegrationTestUpdate" };
            var postReqFilterUpdatedOnDate = new FilterApplicationUserRoleServiceRequest { UpdatedOnDate = todaysUtcDate };
            var postReqFilterApplicationUserRoleIds = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = new List<int> { securityTestData.ActiveApplicationUserRoles[0].ApplicationUserRoleId, securityTestData.ActiveApplicationUserRoles[1].ApplicationUserRoleId, securityTestData.ActiveApplicationUserRoles[2].ApplicationUserRoleId } };
            var postReqFilterApplicationId = new FilterApplicationUserRoleServiceRequest { ApplicationId = applicationId };
            var postReqFilterRoleId = new FilterApplicationUserRoleServiceRequest { RoleId = roleId };
            
            // Act
            var filterCreatedByResult = await _applicationUserRoleLogic.Filter(postReqFilterCreatedBy);
            var filterCreatedOnDateResult = await _applicationUserRoleLogic.Filter(postReqFilterCreatedOnDate);
            var filterUpdatedByResult = await _applicationUserRoleLogic.Filter(postReqFilterUpdatedBy);
            var filterUpdatedOnDateResult = await _applicationUserRoleLogic.Filter(postReqFilterUpdatedOnDate);
            var filterApplicationUserRoleIdsResult = await _applicationUserRoleLogic.Filter(postReqFilterApplicationUserRoleIds);
            var filterApplicationIdResult = await _applicationUserRoleLogic.Filter(postReqFilterApplicationId);
            var filterRoleIdResult = await _applicationUserRoleLogic.Filter(postReqFilterRoleId);
            
            // Assert
            filterCreatedByResult.Response.Should().HaveCount(1);
            filterCreatedOnDateResult.Response.Should().HaveCount(126);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(126);
            filterApplicationUserRoleIdsResult.Response.Should().HaveCount(3);
            filterApplicationIdResult.Response.Should().HaveCount(26);
            filterRoleIdResult.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ArrangeSecurityTestData();

            var postReqInvalidCreatedBy = new FilterApplicationUserRoleServiceRequest { CreatedBy = "asdfasdf" };
            var postReqInvalidCreatedOnDate = new FilterApplicationUserRoleServiceRequest { CreatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidUpdatedBy = new FilterApplicationUserRoleServiceRequest { UpdatedBy = "asdfasdf" };
            var postReqInvalidUpdatedOnDate = new FilterApplicationUserRoleServiceRequest { UpdatedOnDate = new DateOnly(1989, 06, 15) };
            var postReqInvalidApplicationUserRoleIds = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = new List<int> { -1 } };
            var postReqInvalidApplicationId = new FilterApplicationUserRoleServiceRequest { ApplicationId = -1 };
            var postReqInvalidRoleId = new FilterApplicationUserRoleServiceRequest { RoleId = -1 };
            
            // Act
            var invalidCreatedByResult = await _applicationUserRoleLogic.Filter(postReqInvalidCreatedBy);
            var invalidCreatedOnDateResult = await _applicationUserRoleLogic.Filter(postReqInvalidCreatedOnDate);
            var invalidUpdatedByResult = await _applicationUserRoleLogic.Filter(postReqInvalidUpdatedBy);
            var invalidUpdatedOnDateResult = await _applicationUserRoleLogic.Filter(postReqInvalidUpdatedOnDate);
            var invalidApplicationUserRoleIdsResult = await _applicationUserRoleLogic.Filter(postReqInvalidApplicationUserRoleIds);
            var invalidApplicationIdResult = await _applicationUserRoleLogic.Filter(postReqInvalidApplicationId);
            var invalidRoleIdResult = await _applicationUserRoleLogic.Filter(postReqInvalidRoleId);
            
            // Assert
            invalidCreatedByResult.Response.Should().HaveCount(0);
            invalidCreatedOnDateResult.Response.Should().HaveCount(0);
            invalidUpdatedByResult.Response.Should().HaveCount(0);
            invalidUpdatedOnDateResult.Response.Should().HaveCount(0);
            invalidApplicationUserRoleIdsResult.Response.Should().HaveCount(0);
            invalidApplicationIdResult.Response.Should().HaveCount(0);
            invalidRoleIdResult.Response.Should().HaveCount(0);
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
            var role =  await _securityTestUtilities.Role.CreateActiveTestRecords(application[0].ApplicationId, 1);

            var insertReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application[0].ApplicationId,
                ApplicationUserId = applicationUser[0].ApplicationUserId,
                RoleId = role[0].RoleId,
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserRoleLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _roleLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.Should().NotBeNull();
            result.Response.ApplicationId.Should().Be(insertReq.ApplicationId);
            result.Response.ApplicationUserId.Should().Be(insertReq.ApplicationUserId);
            result.Response.RoleId.Should().Be(insertReq.RoleId);
            result.Response.Active.Should().BeTrue();
            result.Response.CreatedBy.Should().Be(TestConstants.CurrentUser);
            result.Response.UpdatedBy.Should().Be(TestConstants.CurrentUser);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Unique_Error()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var applicationUserRole = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();
            var recordToCreate = _securityTestUtilities.ApplicationUserRole.ConvertApplicationUserRoleDtoToInsertUpdateRequest(applicationUserRole);
            
            var expectedUniqueError = _securityTestUtilities.ApplicationUserRole.GetExpectedUniqueFieldErrors();

            // Act
            var result = await _applicationUserRoleLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _roleLogic);

            //Assert
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedUniqueError);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            
            var recordToCreate = new InsertUpdateApplicationUserRoleRequest();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserRole.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserRoleLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _roleLogic);

            // Assert
            result.Errors.Should().HaveCount(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        [Fact]
        public async Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var recordToCreate = _securityTestUtilities.ApplicationUserRole.CreateInsertUpdateRequestWithMaxLengthErrors(1, 1, 1);

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserRole.GetExpectedMaxLengthFieldErrors();

            // Act
            var result = await _applicationUserRoleLogic.Insert(recordToCreate, _applicationLogic, _applicationUserLogic, _roleLogic);

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();   

            var updateReq = new InsertUpdateApplicationUserRoleRequest
            {
                Active = false,
                ApplicationId = recordToUpdate.ApplicationId,
                ApplicationUserId = recordToUpdate.ApplicationUserId,
                RoleId = recordToUpdate.RoleId,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var result = await _applicationUserRoleLogic.Update(recordToUpdate.ApplicationUserRoleId, updateReq, _applicationLogic, _applicationUserLogic, _roleLogic);

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            result.Response.ApplicationId.Should().Be(updateReq.ApplicationId);
            result.Response.ApplicationUserId.Should().Be(updateReq.ApplicationUserId);
            result.Response.RoleId.Should().Be(updateReq.RoleId);
            result.Response.Active.Should().Be(updateReq.Active);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Unique_Error()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();   
            var recordToCopy = securityTestData.ActiveApplicationUserRoles.Skip(1).FirstOrDefault();

            var updateReq = _securityTestUtilities.ApplicationUserRole.ConvertApplicationUserRoleDtoToInsertUpdateRequest(recordToUpdate);
            updateReq.ApplicationId = recordToCopy.ApplicationId;
            updateReq.ApplicationUserId = recordToCopy.ApplicationUserId;
            updateReq.RoleId = recordToCopy.RoleId;

            // Act
            var updateResult = await _applicationUserRoleLogic.Update(recordToUpdate.ApplicationUserRoleId, updateReq, _applicationLogic, _applicationUserLogic, _roleLogic);

            //Assert
            var expectedUniqueApplicationuserRoleError = _securityTestUtilities.ApplicationUserRole.GetExpectedUniqueFieldErrors();

            updateResult.Errors.Should().HaveCount(1);
            updateResult.Errors.Should().BeEquivalentTo(expectedUniqueApplicationuserRoleError);
        }

        [Fact]
        public async Task Default_Update_Should_Not_Update_Record_Required_Field_Errors()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserRole.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserRoleLogic.Update(recordToUpdate.ApplicationUserRoleId, new InsertUpdateApplicationUserRoleRequest(), _applicationLogic, _applicationUserLogic, _roleLogic);

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToDelete = securityTestData.ActiveApplicationUserRoles.FirstOrDefault();   

            // Act
            var result = await _applicationUserRoleLogic.Delete(recordToDelete.ApplicationUserRoleId);
            var getResult = await _applicationUserRoleLogic.GetById(recordToDelete.ApplicationUserRoleId, new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            getResult.Response.Should().BeNull();
        }

        [Fact]
        public async Task Default_Delete_Should_Not_Delete_Record_Invalid_Id()
        {
            // Arrange
            await ClearAllSecurityTestTableData();

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserRole.GetExpectedRecordDoesNotExistErrors();

            // Act
            var result = await _applicationUserRoleLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
