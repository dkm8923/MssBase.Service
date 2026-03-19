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

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class RolePermissionLogicTests : SecurityTestBase,
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
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(125);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ArrangeSecurityTestData();

            // Act
            var result = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(375);
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

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.ActiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveRolePermissions.FirstOrDefault();  

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
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveRolePermissions.FirstOrDefault();  

            // Act
            var result = await _rolePermissionLogic.GetById(testRecord.RolePermissionId, new BaseLogicGet { IncludeInactive = true });

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
            await ArrangeSecurityTestData();

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
            var securityTestData = await ArrangeSecurityTestData();
            var applicationId = securityTestData.ActiveApplications.FirstOrDefault().ApplicationId;
            var roleId = securityTestData.ActiveRoles.Where(x => x.ApplicationId == applicationId).FirstOrDefault().RoleId;
            var permissionId = securityTestData.ActivePermissions.Where(x => x.ApplicationId == applicationId).FirstOrDefault().PermissionId;
            var rolePermissionId = securityTestData.ActiveRolePermissions.FirstOrDefault().RolePermissionId;

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
            var postReqFilterRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { securityTestData.ActiveRolePermissions[0].RolePermissionId, securityTestData.ActiveRolePermissions[1].RolePermissionId, securityTestData.ActiveRolePermissions[2].RolePermissionId } };
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
            filterCreatedOnDateResult.Response.Should().HaveCount(126);
            filterUpdatedByResult.Response.Should().HaveCount(1);
            filterUpdatedOnDateResult.Response.Should().HaveCount(126);
            filterRolePermissionIdsResult.Response.Should().HaveCount(3);
            filterApplicationIdResult.Response.Should().HaveCount(26);
            filterPermissionIdResult.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ArrangeSecurityTestData();

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

        #endregion

        #region Insert

        //securityTestData

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
            var securityTestData = await ArrangeSecurityTestData();
            var rolePermission = securityTestData.ActiveRolePermissions.FirstOrDefault();
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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveRolePermissions.FirstOrDefault();   

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveRolePermissions.FirstOrDefault();   
            var recordToCopy = securityTestData.ActiveRolePermissions.Skip(1).FirstOrDefault();

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveRolePermissions.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.RolePermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _rolePermissionLogic.Update(recordToUpdate.RolePermissionId, new InsertUpdateRolePermissionRequest(), _applicationLogic, _roleLogic, _permissionLogic);

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
            var recordToDelete = securityTestData.ActiveRolePermissions.FirstOrDefault();   

            // Act
            var result = await _rolePermissionLogic.Delete(recordToDelete.RolePermissionId);
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
            var result = await _rolePermissionLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
