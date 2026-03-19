using Contract.Security.Role;
using Dto.Security.Role;
using Dto.Security.Role.Service;
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
    public class RoleServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IRoleService _roleService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public RoleServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _roleService = _serviceProvider.GetService<IRoleService>();
        }

        #region utils

        private async Task CreateRoleCacheKeys()
        {
            var result = await _roleService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });       

            foreach (var record in result.Response) 
            {
                await _roleService.GetById(record.RoleId, new BaseServiceGet());
                await _roleService.Filter(new FilterRoleServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _roleService.Filter(new FilterRoleServiceRequest { CreatedBy = record.CreatedBy });
                await _roleService.Filter(new FilterRoleServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RoleService_GetAll_0_0";

            // Act
            var result = await _roleService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);
            await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "RoleService_GetAll_1_0";

            // Act
            var result = await _roleService.GetAll(new BaseServiceGet { IncludeInactive = true });
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

            var expectedCacheKey = "RoleService_GetAll_0";

            // Act
            var result = await _roleService.GetAll(new BaseServiceGet());
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
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var aplicationUser = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RoleService_GetById_{aplicationUser.RoleId}_0_0";

            // Act
            var result = await _roleService.GetById(aplicationUser.RoleId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var aplicationUser = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId, false);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"RoleService_GetById_{aplicationUser.RoleId}_1_0";

            // Act
            var result = await _roleService.GetById(aplicationUser.RoleId, new BaseServiceGet { IncludeInactive = true });
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
            var result = await _roleService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
           await ClearAllSecurityTestTableData();
           var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
           await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);
           await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId);

           var roleInsertReq = new InsertUpdateRoleRequest
           {
               ApplicationId = application.ApplicationId,
               Name = "Test Name",
               Description = "Test Description",
               Active = true,
               CurrentUser = TestConstants.SpecificCurrentUserForInsert
           };

           var roleRes = await _roleLogic.Insert(roleInsertReq, _applicationLogic);

           roleInsertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _roleLogic.Update(roleRes.Response.RoleId, roleInsertReq, _applicationLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterRoleServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterRoleServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterRoleServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterRoleServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqRoleIds = new FilterRoleServiceRequest { RoleIds = new List<int> { roleRes.Response.RoleId } };
           var postReqName = new FilterRoleServiceRequest { Name = "Test Name" };
           var postReqApplicationId = new FilterRoleServiceRequest { ApplicationId = application.ApplicationId };
           var postReqIncludeInactive = new FilterRoleServiceRequest { IncludeInactive = true };
           var postReqIncludeRelated = new FilterRoleServiceRequest { IncludeRelated = true };
           
           var expectedCacheKeyCreatedBy = $"RoleService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"RoleService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"RoleService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"RoleService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0";
           var expectedCacheKeyRoleIdsKey = $"RoleService_Filter_0_0_0_0_{(postReqRoleIds.RoleIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0";
           var expectedCacheKeyName = $"RoleService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqName.Name)}_0_0_0";
           var expectedCacheKeyApplicationId = $"RoleService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0";
           var expectedCacheKeyIncludeInactive = $"RoleService_Filter_0_0_0_0_0_0_0_1_0";
           var expectedCacheKeyIncludeRelated = $"RoleService_Filter_0_0_0_0_0_0_0_0_1";
           
            // Act
           var filterCreatedByResult = await _roleService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _roleService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _roleService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _roleService.Filter(postReqUpdatedOnDate);
           var filterRoleIdsResult = await _roleService.Filter(postReqRoleIds);
           var filterNameResult = await _roleService.Filter(postReqName);
           var filterApplicationIdResult = await _roleService.Filter(postReqApplicationId);
           var filterIncludeInactiveResult = await _roleService.Filter(postReqIncludeInactive);
           var filterIncludeRelatedResult = await _roleService.Filter(postReqIncludeRelated);

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

           availableCacheKeys.Should().Contain(expectedCacheKeyRoleIdsKey);
           filterRoleIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyName);
           filterNameResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(6);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeInactive);
           filterIncludeInactiveResult.Response.Should().HaveCount(11);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeRelated);
           filterIncludeRelatedResult.Response.Should().HaveCount(6);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRoleCacheKeys();

            var insertReq = _securityTestUtilities.Role.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await _roleService.Insert(insertReq);
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
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRoleCacheKeys();

            var updateReq = new InsertUpdateRoleRequest
            {
                Name = "Name Updated",
                Description = "Description Updated",
                ApplicationId = application.ApplicationId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var result = await _roleService.Update(testRecord.RoleId, updateReq);
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
            var testRecord = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateRoleCacheKeys();

            // Act
            await _roleService.Delete(testRecord.RoleId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
