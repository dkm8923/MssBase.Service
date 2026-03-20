using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
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
    public class RolePermissionServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public RolePermissionServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _rolePermissionService = _serviceProvider.GetService<IRolePermissionService>();
        }

        #region utils
        
        private async Task CreateRolePermissionCacheKeys()
        {
            var result = await _rolePermissionService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });       

            foreach (var record in result.Response) 
            {
                await _rolePermissionService.GetById(record.RolePermissionId, new BaseServiceGet());
                await _rolePermissionService.Filter(new FilterRolePermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _rolePermissionService.Filter(new FilterRolePermissionServiceRequest { CreatedBy = record.CreatedBy });
                await _rolePermissionService.Filter(new FilterRolePermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            await ArrangeRolePermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RolePermissionService_GetAll_0_0";

            // Act
            var result = await _rolePermissionService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ArrangeRolePermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "RolePermissionService_GetAll_1_0";

            // Act
            var result = await _rolePermissionService.GetAll(new BaseServiceGet { IncludeInactive = true });
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

            var expectedCacheKey = "RolePermissionService_GetAll_0";

            // Act
            var result = await _rolePermissionService.GetAll(new BaseServiceGet());
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
            var securityTestData = await ArrangeRolePermissionTestData();
            var activeRolePermission = securityTestData.ActiveRolePermissions.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RolePermissionService_GetById_{activeRolePermission.RolePermissionId}_0_0";

            // Act
            var result = await _rolePermissionService.GetById(activeRolePermission.RolePermissionId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeRolePermissionTestData();
            var inactiveRolePermission = securityTestData.InactiveRolePermissions.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RolePermissionService_GetById_{inactiveRolePermission.RolePermissionId}_1_0";

            // Act
            var result = await _rolePermissionService.GetById(inactiveRolePermission.RolePermissionId, new BaseServiceGet { IncludeInactive = true });
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
            var result = await _rolePermissionService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
            var securityTestData = await ArrangeRolePermissionTestData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
           
            var insertReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert,
                Active = true
            };

           var rolePermissionRes = await _rolePermissionLogic.Insert(insertReq, _applicationLogic, _roleLogic, _permissionLogic);

           insertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _rolePermissionLogic.Update(rolePermissionRes.Response.RolePermissionId, insertReq, _applicationLogic, _roleLogic, _permissionLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterRolePermissionServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterRolePermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterRolePermissionServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterRolePermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqRolePermissionIds = new FilterRolePermissionServiceRequest { RolePermissionIds = new List<int> { rolePermissionRes.Response.RolePermissionId } };
           var postReqApplicationId = new FilterRolePermissionServiceRequest { ApplicationId = application.ApplicationId };
           var postReqRoleId = new FilterRolePermissionServiceRequest { RoleId = role.RoleId };
           var postReqPermissionId = new FilterRolePermissionServiceRequest { PermissionId = permission.PermissionId };
           var postReqIncludeInactive = new FilterRolePermissionServiceRequest { IncludeInactive = true };
           
           var expectedCacheKeyCreatedBy = $"RolePermissionService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"RolePermissionService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"RolePermissionService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"RolePermissionService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0";
           var expectedCacheKeyRolePermissionIdsKey = $"RolePermissionService_Filter_0_0_0_0_{(postReqRolePermissionIds.RolePermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0";
           var expectedCacheKeyApplicationId = $"RolePermissionService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0_0";
           var expectedCacheKeyRoleId = $"RolePermissionService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqRoleId.RoleId.ToString())}_0_0";
           var expectedCacheKeyPermissionId = $"RolePermissionService_Filter_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqPermissionId.PermissionId.ToString())}_0";
           var expectedCacheKeyIncludeInactive = $"RolePermissionService_Filter_0_0_0_0_0_0_0_0_1";
           
           // Act
           var filterCreatedByResult = await _rolePermissionService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _rolePermissionService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _rolePermissionService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _rolePermissionService.Filter(postReqUpdatedOnDate);
           var filterRolePermissionIdsResult = await _rolePermissionService.Filter(postReqRolePermissionIds);
           var filterApplicationIdResult = await _rolePermissionService.Filter(postReqApplicationId);
           var filterRoleIdResult = await _rolePermissionService.Filter(postReqRoleId);
           var filterPermissionIdResult = await _rolePermissionService.Filter(postReqPermissionId);
           var filterIncludeInactiveResult = await _rolePermissionService.Filter(postReqIncludeInactive);
           
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

           availableCacheKeys.Should().Contain(expectedCacheKeyRolePermissionIdsKey);
           filterRolePermissionIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyRoleId);
           filterRoleIdResult.Response.Should().HaveCount(1);

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRolePermissionCacheKeys();

            var insertReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var insertResult = await _rolePermissionService.Insert(insertReq);
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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var rolePermission = await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRolePermissionCacheKeys();

            var updateReq = new InsertUpdateRolePermissionRequest
            {
                ApplicationId = application.ApplicationId,
                RoleId = role.RoleId,
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.CurrentUser,
                Active = false
            };

            // Act
            var result = await _rolePermissionService.Update(rolePermission.RolePermissionId, updateReq);
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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var permission = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            var rolePermission = await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, permission.PermissionId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRolePermissionCacheKeys();

            // Act
            await _rolePermissionService.Delete(rolePermission.RolePermissionId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
