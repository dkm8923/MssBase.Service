using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
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
    public class PermissionServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IPermissionService _permissionService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public PermissionServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _permissionService = _serviceProvider.GetService<IPermissionService>();
        }

        #region utils

        private async Task CreatePermissionCacheKeys()
        {
            var result = await _permissionService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });       

            foreach (var record in result.Response) 
            {
                await _permissionService.GetById(record.PermissionId, new BaseServiceGet());
                await _permissionService.Filter(new FilterPermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _permissionService.Filter(new FilterPermissionServiceRequest { CreatedBy = record.CreatedBy });
                await _permissionService.Filter(new FilterPermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"PermissionService_GetAll_0_0";

            // Act
            var result = await _permissionService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "PermissionService_GetAll_1_0";

            // Act
            var result = await _permissionService.GetAll(new BaseServiceGet { IncludeInactive = true });
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

            var expectedCacheKey = "PermissionService_GetAll_0";

            // Act
            var result = await _permissionService.GetAll(new BaseServiceGet());
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
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var permission = arrangeTestDataResponse.ActivePermissions.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"PermissionService_GetById_{permission.PermissionId}_0_0";

            // Act
            var result = await _permissionService.GetById(permission.PermissionId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangePermissionTestData();
            var permission = arrangeTestDataResponse.InactivePermissions.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"PermissionService_GetById_{permission.PermissionId}_1_0";

            // Act
            var result = await _permissionService.GetById(permission.PermissionId, new BaseServiceGet { IncludeInactive = true });
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
            var result = await _permissionService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
           await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
           await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId);

           var permissionInsertReq = new InsertUpdatePermissionRequest
           {
               ApplicationId = application.ApplicationId,
               Name = "Test Name",
               Description = "Test Description",
               Active = true,
               CurrentUser = TestConstants.SpecificCurrentUserForInsert
           };

           var permissionRes = await _permissionLogic.Insert(permissionInsertReq, _applicationLogic);

           permissionInsertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _permissionLogic.Update(permissionRes.Response.PermissionId, permissionInsertReq, _applicationLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterPermissionServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterPermissionServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterPermissionServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterPermissionServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqPermissionIds = new FilterPermissionServiceRequest { PermissionIds = new List<int> { permissionRes.Response.PermissionId } };
           var postReqName = new FilterPermissionServiceRequest { Name = "Test Name" };
           var postReqApplicationId = new FilterPermissionServiceRequest { ApplicationId = application.ApplicationId };
           var postReqIncludeInactive = new FilterPermissionServiceRequest { IncludeInactive = true };
           
           var expectedCacheKeyCreatedBy = $"PermissionService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"PermissionService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"PermissionService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"PermissionService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0";
           var expectedCacheKeyPermissionIdsKey = $"PermissionService_Filter_0_0_0_0_{(postReqPermissionIds.PermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0";
           var expectedCacheKeyName = $"PermissionService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqName.Name)}_0_0";
           var expectedCacheKeyApplicationId = $"PermissionService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0";
           var expectedCacheKeyIncludeInactive = $"PermissionService_Filter_0_0_0_0_0_0_0_1";
           
            // Act
           var filterCreatedByResult = await _permissionService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _permissionService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _permissionService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _permissionService.Filter(postReqUpdatedOnDate);
           var filterPermissionIdsResult = await _permissionService.Filter(postReqPermissionIds);
           var filterNameResult = await _permissionService.Filter(postReqName);
           var filterApplicationIdResult = await _permissionService.Filter(postReqApplicationId);
           var filterIncludeInactiveResult = await _permissionService.Filter(postReqIncludeInactive);
           
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

           availableCacheKeys.Should().Contain(expectedCacheKeyPermissionIdsKey);
           filterPermissionIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyName);
           filterNameResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(6);

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
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreatePermissionCacheKeys();

            var insertReq = _securityTestUtilities.Permission.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await _permissionService.Insert(insertReq);
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
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreatePermissionCacheKeys();

            var updateReq = new InsertUpdatePermissionRequest
            {
                Name = "Name Updated",
                Description = "Description Updated",
                ApplicationId = application.ApplicationId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var result = await _permissionService.Update(testRecord.PermissionId, updateReq);
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
            var testRecord = await _securityTestUtilities.Permission.CreateSinglePermissionTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreatePermissionCacheKeys();

            // Act
            await _permissionService.Delete(testRecord.PermissionId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
