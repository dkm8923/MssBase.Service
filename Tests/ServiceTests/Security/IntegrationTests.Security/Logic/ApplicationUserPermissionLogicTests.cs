using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Dto.Security.ApplicationUserPermission.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using Shared.Models;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities.Contracts.Logic;
using IntegrationTests.Shared.Utilities;

namespace IntegrationTests.Security.Logic
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserPermissionLogicTests : SecurityTestBase,
                                                       IDefaultLogicTestsGetAll,
                                                       IDefaultLogicTestsGetById,
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
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet());

            // Assert
            result.Response.Should().HaveCount(125);
        }

        [Fact]
        public async Task Default_GetAll_Should_Return_Inactive_Data()
        {
            // Arrange
            await ArrangeSecurityTestData();

            // Act
            var result = await _applicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

            // Assert
            result.Response.Should().HaveCount(375);
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

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Return_Active_Record()
        {
            // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.ActiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet());

            // Assert
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Should_Not_Return_Inactive_Record()
        {
           // Arrange
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveApplicationUserPermissions.FirstOrDefault();  

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
            var securityTestData = await ArrangeSecurityTestData();
            var testRecord = securityTestData.InactiveApplicationUserPermissions.FirstOrDefault();  

            // Act
            var result = await _applicationUserPermissionLogic.GetById(testRecord.ApplicationUserPermissionId, new BaseLogicGet { IncludeInactive = true });

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
            await ArrangeSecurityTestData();

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
        public async Task Default_Filter_Should_Return_Zero_Records()
        {
            // Arrange
            await ArrangeSecurityTestData();

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
            var securityTestData = await ArrangeSecurityTestData();
            var applicationId = securityTestData.ActiveApplications[0].ApplicationId;
            var applicationUserId = securityTestData.ActiveApplicationUsers.FirstOrDefault(r => r.ApplicationId == applicationId).ApplicationUserId;
            var permissionId = securityTestData.ActivePermissions.FirstOrDefault(r => r.ApplicationId == applicationId).PermissionId;   

            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = applicationId,
                ApplicationUserId = applicationUserId,
                PermissionId = permissionId,
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
            var securityTestData = await ArrangeSecurityTestData();
            var applicationId = securityTestData.ActiveApplications[0].ApplicationId;
            var applicationUserId = securityTestData.ActiveApplicationUsers.FirstOrDefault(r => r.ApplicationId == applicationId).ApplicationUserId;
            var permissionId = securityTestData.ActivePermissions.FirstOrDefault(r => r.ApplicationId == applicationId).PermissionId;   

            var testRecord = await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(applicationId, applicationUserId, permissionId, 1);

            var recordToCreate = _securityTestUtilities.ApplicationUserPermission.ConvertApplicationUserPermissionDtoToInsertUpdateRequest(testRecord.FirstOrDefault());

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserPermissions.FirstOrDefault();   

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserPermissions.FirstOrDefault();   
            var recordToCopy = securityTestData.ActiveApplicationUserPermissions.Skip(1).FirstOrDefault();

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
            var securityTestData = await ArrangeSecurityTestData();
            var recordToUpdate = securityTestData.ActiveApplicationUserPermissions.FirstOrDefault();   

            var expectedFieldErrors = _securityTestUtilities.ApplicationUserPermission.GetExpectedRequiredFieldErrors();

            // Act
            var result = await _applicationUserPermissionLogic.Update(recordToUpdate.ApplicationUserPermissionId, new InsertUpdateApplicationUserPermissionRequest(), _applicationLogic, _applicationUserLogic, _permissionLogic);

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
            var recordToDelete = securityTestData.ActiveApplicationUserPermissions.FirstOrDefault();   

            // Act
            var result = await _applicationUserPermissionLogic.Delete(recordToDelete.ApplicationUserPermissionId);
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
            var result = await _applicationUserPermissionLogic.Delete(-1);

            // Assert
            result.Errors.Count.Should().Be(expectedFieldErrors.Count);

            LogicTestUtilities.VerifyLogicErrorResultsAreValid(expectedFieldErrors, result.Errors);
        }

        #endregion
    }
}
