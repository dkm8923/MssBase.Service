using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Logic.Common;
using Shared.Models;

namespace IntegrationTests.Security.Service
{
    [Collection("SecurityIntegrationTests")]
    public class ApplicationUserServiceTests : SecurityTestBase
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public ApplicationUserServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _applicationUserService = _serviceProvider.GetService<IApplicationUserService>();
        }

        #region GetAll

        [Fact]
        public async Task ApplicationUser_GetAll_Active_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
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
        public async Task ApplicationUser_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_1_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task ApplicationUser_GetById_Should_Cache()
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

        #endregion

        #region Filter

        [Fact]
        public async Task ApplicationUser_Filter_Should_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
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
        public async Task ApplicationUser_Insert_Should_Clear_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            await _securityTestUtilities.ApplicationUser.CreateTestRecords(applicationId);
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
        public async Task ApplicationUser_Update_Should_Clear_Cache()
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

        [Fact]
        public async Task ApplicationUser_Delete_Should_Clear_Cache()
        {
            // Arrange
            int applicationId = await _securityTestUtilities.ApplicationUser.ClearTestTablesAndReturnApplicationId(_securityTestUtilities.Application);
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(applicationId);
            await _cacheTestUtilities.DeleteAllKeyData();

            // Act
            var result = await _applicationUserService.Delete(testRecord.ApplicationUserId);
            var cacheKeysAfter = _cacheTestUtilities.GetKeys();

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            cacheKeysAfter.Should().HaveCount(0);
        }

        #endregion
    }
}
