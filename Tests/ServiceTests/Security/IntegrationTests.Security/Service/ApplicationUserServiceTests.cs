using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
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
    public class ApplicationUserServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsFilter//,
                                            //    IDefaultServiceTestsInsert,
                                            //    IDefaultServiceTestsUpdate,
                                            //    IDefaultServiceTestsDelete
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public ApplicationUserServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _applicationUserService = _serviceProvider.GetService<IApplicationUserService>();
        }

        #region utils

        private async Task CreateApplicationUserCacheKeys()
        {
            var result = await _applicationUserService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });

            foreach (var record in result.Response)
            {
                await _applicationUserService.GetById(record.ApplicationUserId, new BaseServiceGet());
                //await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { Name = record.Name });
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { CreatedOnDate = DateOnly.Parse(record.CreatedOn.ToString()) });
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { CreatedBy = record.CreatedBy });
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { UpdatedOnDate = DateOnly.Parse(record.UpdatedOn.ToString()) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetAll_0_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_1_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Cache_And_Return_Zero_Records()
        {
            // Arrange
            await _securityTestUtilities.ApplicationUser.DeleteAllRecords();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet());
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
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);
            var expectedCacheKey = $"ApplicationUserService_GetById_{testRecord.ApplicationUserId}_0_0";

            // Act
            var result = await _applicationUserService.GetById(testRecord.ApplicationUserId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);
            var expectedCacheKey = $"ApplicationUserService_GetById_{testRecord.ApplicationUserId}_1_0";

            // Act
            var result = await _applicationUserService.GetById(testRecord.ApplicationUserId, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_Unused_Id_Should_Not_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var id = -1;

            // Act
            var result = await _applicationUserService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var postReq = new FilterApplicationUserServiceRequest { CreatedBy = TestConstants.CurrentUser };

            // Act
            var result = await _applicationUserService.Filter(postReq);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().HaveCountGreaterThan(0);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Clear_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(applicationId);
            var applicationUserCacheKeys = _cacheTestUtilities.GetKeys();
            applicationUserCacheKeys.Should().HaveCountGreaterThan(0);

            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(applicationId);

            // Act
            var result = await _applicationUserService.Insert(insertReq);
            var cacheKeysAfterInsert = _cacheTestUtilities.GetKeys();

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            var remainingApplicationUserKeys = cacheKeysAfterInsert.Where(k => k.Contains("ApplicationUserService")).ToList();
            remainingApplicationUserKeys.Should().HaveCount(0);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Clear_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "Updated",
                LastName = "User",
                ApplicationId = applicationId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var result = await _applicationUserService.Update(testRecord.ApplicationUserId, updateReq);
            var cacheKeysAfter = _cacheTestUtilities.GetKeys();

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            cacheKeysAfter.Should().HaveCount(0);
        }

        #endregion

        #region Delete

        // [Fact]
        // public async Task Default_Delete_Should_Delete_Cache()
        // {
        //     // Arrange
        //     await ClearAllSecurityTestTableData();
        //     await _cacheTestUtilities.DeleteAllKeyData();
        //     await CreateApplicationUserCacheKeys();

        //     var record = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord();

        //     // Act
        //     await _applicationUserService.Delete(record.ApplicationUserId);
        //     var availableCacheKeys = _cacheTestUtilities.GetKeys();

        //     //Assert
        //     availableCacheKeys.Should().HaveCount(0);
        // }

        #endregion
    }
}
