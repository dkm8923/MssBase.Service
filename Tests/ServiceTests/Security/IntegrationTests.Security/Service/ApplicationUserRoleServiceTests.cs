using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
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
    public class ApplicationUserRoleServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IApplicationUserRoleService _applicationUserRoleService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public ApplicationUserRoleServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _applicationUserRoleService = _serviceProvider.GetService<IApplicationUserRoleService>();
        }

        #region utils
        
        private async Task CreateApplicationUserRoleCacheKeys()
        {
            var result = await _applicationUserRoleService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });       

            foreach (var record in result.Response) 
            {
                await _applicationUserRoleService.GetById(record.ApplicationUserRoleId, new BaseServiceGet());
                await _applicationUserRoleService.Filter(new FilterApplicationUserRoleServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _applicationUserRoleService.Filter(new FilterApplicationUserRoleServiceRequest { CreatedBy = record.CreatedBy });
                await _applicationUserRoleService.Filter(new FilterApplicationUserRoleServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            await ArrangeApplicationUserRoleTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserRoleService_GetAll_0_0";

            // Act
            var result = await _applicationUserRoleService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ArrangeApplicationUserRoleTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserRoleService_GetAll_1_0";

            // Act
            var result = await _applicationUserRoleService.GetAll(new BaseServiceGet { IncludeInactive = true });
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

            var expectedCacheKey = "ApplicationUserRoleService_GetAll_0";

            // Act
            var result = await _applicationUserRoleService.GetAll(new BaseServiceGet());
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
            var securityTestData = await ArrangeApplicationUserRoleTestData();
            var activeApplicationUserRole = securityTestData.ActiveApplicationUserRoles.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserRoleService_GetById_{activeApplicationUserRole.ApplicationUserRoleId}_0_0";

            // Act
            var result = await _applicationUserRoleService.GetById(activeApplicationUserRole.ApplicationUserRoleId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeApplicationUserRoleTestData();
            var inactiveApplicationUserRole = securityTestData.InactiveApplicationUserRoles.First();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserRoleService_GetById_{inactiveApplicationUserRole.ApplicationUserRoleId}_1_0";

            // Act
            var result = await _applicationUserRoleService.GetById(inactiveApplicationUserRole.ApplicationUserRoleId, new BaseServiceGet { IncludeInactive = true });
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
            var result = await _applicationUserRoleService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
            var securityTestData = await ArrangeApplicationUserRoleTestData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
           
            var insertReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = role.RoleId,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert,
                Active = true
            };

           var applicationUserRoleRes = await _applicationUserRoleLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _roleLogic);

           insertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _applicationUserRoleLogic.Update(applicationUserRoleRes.Response.ApplicationUserRoleId, insertReq, _applicationLogic, _applicationUserLogic, _roleLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterApplicationUserRoleServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterApplicationUserRoleServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterApplicationUserRoleServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterApplicationUserRoleServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqApplicationUserRoleIds = new FilterApplicationUserRoleServiceRequest { ApplicationUserRoleIds = new List<int> { applicationUserRoleRes.Response.ApplicationUserRoleId } };
           var postReqApplicationId = new FilterApplicationUserRoleServiceRequest { ApplicationId = application.ApplicationId };
           var postReqApplicationUserId = new FilterApplicationUserRoleServiceRequest { ApplicationUserId = applicationUser.ApplicationUserId };
           var postReqRoleId = new FilterApplicationUserRoleServiceRequest { RoleId = role.RoleId };
           var postReqIncludeInactive = new FilterApplicationUserRoleServiceRequest { IncludeInactive = true };
           
           var expectedCacheKeyCreatedBy = $"ApplicationUserRoleService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"ApplicationUserRoleService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"ApplicationUserRoleService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"ApplicationUserRoleService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0";
           var expectedCacheKeyApplicationUserRoleIdsKey = $"ApplicationUserRoleService_Filter_0_0_0_0_{(postReqApplicationUserRoleIds.ApplicationUserRoleIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0";
           var expectedCacheKeyApplicationId = $"ApplicationUserRoleService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0_0";
           var expectedCacheKeyApplicationUserId = $"ApplicationUserRoleService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationUserId.ApplicationUserId.ToString())}_0_0";
           var expectedCacheKeyRoleId = $"ApplicationUserRoleService_Filter_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqRoleId.RoleId.ToString())}_0";
           var expectedCacheKeyIncludeInactive = $"ApplicationUserRoleService_Filter_0_0_0_0_0_0_0_0_1";
           
           // Act
           var filterCreatedByResult = await _applicationUserRoleService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _applicationUserRoleService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _applicationUserRoleService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _applicationUserRoleService.Filter(postReqUpdatedOnDate);
           var filterApplicationUserRoleIdsResult = await _applicationUserRoleService.Filter(postReqApplicationUserRoleIds);
           var filterApplicationIdResult = await _applicationUserRoleService.Filter(postReqApplicationId);
           var filterApplicationUserIdResult = await _applicationUserRoleService.Filter(postReqApplicationUserId);
           var filterRoleIdResult = await _applicationUserRoleService.Filter(postReqRoleId);
           var filterIncludeInactiveResult = await _applicationUserRoleService.Filter(postReqIncludeInactive);
           
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

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserRoleIdsKey);
           filterApplicationUserRoleIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserId);
           filterApplicationUserIdResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyRoleId);
           filterRoleIdResult.Response.Should().HaveCount(1);

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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserRoleCacheKeys();

            var insertReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = role.RoleId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var insertResult = await _applicationUserRoleService.Insert(insertReq);
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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var applicationUserRole = await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, role.RoleId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserRoleCacheKeys();

            var updateReq = new InsertUpdateApplicationUserRoleRequest
            {
                ApplicationId = application.ApplicationId,
                ApplicationUserId = applicationUser.ApplicationUserId,
                RoleId = role.RoleId,
                CurrentUser = TestConstants.CurrentUser,
                Active = false
            };

            // Act
            var result = await _applicationUserRoleService.Update(applicationUserRole.ApplicationUserRoleId, updateReq);
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
            var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
            var applicationUserRole = await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, role.RoleId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserRoleCacheKeys();

            // Act
            await _applicationUserRoleService.Delete(applicationUserRole.ApplicationUserRoleId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
