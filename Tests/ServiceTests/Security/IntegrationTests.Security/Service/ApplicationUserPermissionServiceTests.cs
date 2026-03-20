using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using IntegrationTests.Shared.Utilities.Contracts.Service;
using Microsoft.Extensions.DependencyInjection;
using Shared.Logic.Common;
using Shared.Models;

namespace IntegrationTests.Security.Service
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserPermissionServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IApplicationUserPermissionService _applicationUserPermissionService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public ApplicationUserPermissionServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _applicationUserPermissionService = _serviceProvider.GetService<IApplicationUserPermissionService>();
        }

        #region utils
        
        private async Task CreateApplicationUserPermissionCacheKeys()
        {
            var result = await _applicationUserPermissionService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });       

            foreach (var record in result.Response) 
            {
                await _applicationUserPermissionService.GetById(record.ApplicationUserPermissionId, new BaseServiceGet());
                await _applicationUserPermissionService.Filter(new FilterApplicationUserPermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _applicationUserPermissionService.Filter(new FilterApplicationUserPermissionServiceRequest { CreatedBy = record.CreatedBy });
                await _applicationUserPermissionService.Filter(new FilterApplicationUserPermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            await ArrangeApplicationUserPermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserPermissionService_GetAll_0_0";

            // Act
            var result = await _applicationUserPermissionService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ArrangeApplicationUserPermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserPermissionService_GetAll_1_0";

            // Act
            var result = await _applicationUserPermissionService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(10);
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Cache_And_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserPermissionService_GetAll_0";

            // Act
            var result = await _applicationUserPermissionService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().NotContain(expectedCacheKey);
            result.Response.Should().HaveCount(0);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task Default_GetById_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeApplicationUserPermissionTestData();
            var activeApplicationUserPermission = securityTestData.ActiveApplicationUserPermissions.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserPermissionService_GetById_{activeApplicationUserPermission.ApplicationUserPermissionId}_0_0";

            // Act
            var result = await _applicationUserPermissionService.GetById(activeApplicationUserPermission.ApplicationUserPermissionId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeApplicationUserPermissionTestData();
            var inactiveApplicationUserPermission = securityTestData.InactiveApplicationUserPermissions.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserPermissionService_GetById_{inactiveApplicationUserPermission.ApplicationUserPermissionId}_1_0";

            // Act
            var result = await _applicationUserPermissionService.GetById(inactiveApplicationUserPermission.ApplicationUserPermissionId, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Unused_Id_Should_Not_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var id = -1;

            // Act
            var result = await _applicationUserPermissionService.GetById(id, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            result.Response.Should().BeNull();
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeApplicationUserPermissionTestData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
           
            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert,
                Active = true
            };

           var applicationUserPermissionRes = await _applicationUserPermissionLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

           insertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _applicationUserPermissionLogic.Update(applicationUserPermissionRes.Response.ApplicationUserPermissionId, insertReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterApplicationUserPermissionServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterApplicationUserPermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterApplicationUserPermissionServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterApplicationUserPermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqApplicationUserPermissionIds = new FilterApplicationUserPermissionServiceRequest { ApplicationUserPermissionIds = new List<int> { applicationUserPermissionRes.Response.ApplicationUserPermissionId } };
           var postReqApplicationId = new FilterApplicationUserPermissionServiceRequest { ApplicationId = application.ApplicationId };
           var postReqApplicationUserId = new FilterApplicationUserPermissionServiceRequest { ApplicationUserId = applicationUser.ApplicationUserId };
           var postReqPermissionId = new FilterApplicationUserPermissionServiceRequest { PermissionId = permission.PermissionId };
           var postReqIncludeInactive = new FilterApplicationUserPermissionServiceRequest { IncludeInactive = true };
           
           var expectedCacheKeyCreatedBy = $"ApplicationUserPermissionService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"ApplicationUserPermissionService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"ApplicationUserPermissionService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"ApplicationUserPermissionService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0";
           var expectedCacheKeyApplicationUserPermissionIdsKey = $"ApplicationUserPermissionService_Filter_0_0_0_0_{(postReqApplicationUserPermissionIds.ApplicationUserPermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0";
           var expectedCacheKeyApplicationId = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0_0";
           var expectedCacheKeyApplicationUserId = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationUserId.ApplicationUserId.ToString())}_0_0";
           var expectedCacheKeyPermissionId = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqPermissionId.PermissionId.ToString())}_0";
           var expectedCacheKeyIncludeInactive = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_0_0_1";
           
           // Act
           var filterCreatedByResult = await _applicationUserPermissionService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _applicationUserPermissionService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _applicationUserPermissionService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _applicationUserPermissionService.Filter(postReqUpdatedOnDate);
           var filterApplicationUserPermissionIdsResult = await _applicationUserPermissionService.Filter(postReqApplicationUserPermissionIds);
           var filterApplicationIdResult = await _applicationUserPermissionService.Filter(postReqApplicationId);
           var filterApplicationUserIdResult = await _applicationUserPermissionService.Filter(postReqApplicationUserId);
           var filterPermissionIdResult = await _applicationUserPermissionService.Filter(postReqPermissionId);
           var filterIncludeInactiveResult = await _applicationUserPermissionService.Filter(postReqIncludeInactive);
           
           var availableCacheKeys = _cacheTestUtilities.GetKeys();

           // Assert
           availableCacheKeys.Should().Contain(expectedCacheKeyCreatedBy);
           filterCreatedByResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyCreatedOnDate);
           filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyUpdatedBy);
           filterUpdatedByResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyUpdatedOnDate);
           filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserPermissionIdsKey);
           filterApplicationUserPermissionIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserId);
           filterApplicationUserIdResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyPermissionId);
           filterPermissionIdResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeInactive);
           filterIncludeInactiveResult.Response.Should().HaveCount(11);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserPermissionCacheKeys();

            var insertReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var insertResult = await _applicationUserPermissionService.Insert(insertReq);
            var cacheKeysAfterInsert = _cacheTestUtilities.GetKeys();

            // Assert
            insertResult.Errors.Should().BeNullOrEmpty();
            cacheKeysAfterInsert.Should().HaveCount(0);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var applicationUserPermission = await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserPermissionCacheKeys();

            var updateReq = new InsertUpdateApplicationUserPermissionRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.CurrentUser,
                Active = false
            };

            // Act
            var result = await _applicationUserPermissionService.Update(applicationUserPermission.ApplicationUserPermissionId, updateReq);
            var cacheKeysAfter = _cacheTestUtilities.GetKeys();

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            cacheKeysAfter.Should().HaveCount(0);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var applicationUserPermission = await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, permission.PermissionId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserPermissionCacheKeys();

            // Act
            await _applicationUserPermissionService.Delete(applicationUserPermission.ApplicationUserPermissionId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
