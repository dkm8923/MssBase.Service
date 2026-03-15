using Contract.Security;
using Contract.Security.Application;
using Dto.Security.Application;
using Dto.Security.Application.Service;
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
    public class ApplicationServiceTests : SecurityTestBase,
                                           IDefaultServiceTestsGetAll,
                                           IDefaultServiceTestsGetById,
                                           IDefaultServiceTestsFilter,
                                           IDefaultServiceTestsInsert,
                                           IDefaultServiceTestsUpdate,
                                           IDefaultServiceTestsDelete
    {
        //TODO: Add tests for include related logic
        private readonly IApplicationService _applicationService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public ApplicationServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _applicationService = _serviceProvider.GetService<IApplicationService>();
        }

        #region utils

        private async Task CreateApplicationCacheKeys()
        {
            var result = await _applicationService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });

            foreach (var record in result.Response)
            {
                await _applicationService.GetById(record.ApplicationId, new BaseServiceGet());
                await _applicationService.Filter(new FilterApplicationServiceRequest { Name = record.Name });
                await _applicationService.Filter(new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.Parse(record.CreatedOn.ToString()) });
                await _applicationService.Filter(new FilterApplicationServiceRequest { CreatedBy = record.CreatedBy });
                await _applicationService.Filter(new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.Parse(record.UpdatedOn.ToString()) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationService_GetAll_0_0";

            // Act
            var result = await _applicationService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();
            var cacheKeyData = await _cacheTestUtilities.GetKeyData<List<ApplicationDto>>(expectedCacheKey);

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationService_GetAll_1_0";

            // Act
            var result = await _applicationService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();
            var cacheKeyData = await _cacheTestUtilities.GetKeyData<List<ApplicationDto>>(expectedCacheKey);

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(5);
        }

        [Fact]
        public async Task Default_GetAll_Should_Not_Cache_And_Return_Zero_Records()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationService_GetAll_0";

            // Act
            var result = await _applicationService.GetAll(new BaseServiceGet());
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
            await _cacheTestUtilities.DeleteAllKeyData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var expectedCacheKey = $"ApplicationService_GetById_{testRecord.ApplicationId}_0_0";

            // Act
            var result = await _applicationService.GetById(testRecord.ApplicationId, new BaseServiceGet());
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
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord(false);
            var expectedCacheKey = $"ApplicationService_GetById_{testRecord.ApplicationId}_1_0";

            // Act
            var result = await _applicationService.GetById(testRecord.ApplicationId, new BaseServiceGet { IncludeInactive = true });
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
            await _securityTestUtilities.Application.CreateActiveTestRecords();
            await _cacheTestUtilities.DeleteAllKeyData();

            var id = -1;

            // Act
            var result = await _applicationService.GetById(id, new BaseServiceGet { IncludeInactive = true });
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
           await _securityTestUtilities.Application.CreateActiveTestRecords();
           await _securityTestUtilities.Application.CreateSingleApplicationTestRecordWithSpecificValues();
           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser};
           var postReqCreatedOnDate = new FilterApplicationServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterApplicationServiceRequest { UpdatedBy = TestConstants.CurrentUser};
           var postReqUpdatedOnDate = new FilterApplicationServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqName = new FilterApplicationServiceRequest { Name = "Test Application Name" };
           
           var expectedCacheKeyCreatedBy = $"ApplicationService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"ApplicationService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"ApplicationService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"ApplicationService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0";
           var expectedCacheKeyName = $"ApplicationService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqName.Name)}_0";

           // Act
           var filterCreatedByResult = await _applicationService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _applicationService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _applicationService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _applicationService.Filter(postReqUpdatedOnDate);
           var filterNameResult = await _applicationService.Filter(postReqName);
           var availableCacheKeys = _cacheTestUtilities.GetKeys();

           // Assert
           availableCacheKeys.Should().Contain(expectedCacheKeyCreatedBy);
           filterCreatedByResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyCreatedOnDate);
           filterCreatedOnDateResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyUpdatedBy);
           filterUpdatedByResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyUpdatedOnDate);
           filterUpdatedOnDateResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyName);
           filterNameResult.Response.Should().HaveCount(1);
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationCacheKeys();

            var insertReq = new InsertUpdateApplicationRequest
            {
                Name = "Test Application Name 1",
                Description = "Test Application Desc 1",
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            await _applicationService.Insert(insertReq);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Default_Update_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationCacheKeys();

            var record = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            var updateReq = new InsertUpdateApplicationRequest
            {
                Name = "Updated Application Name",
                Description = "Updated ApplicationDescription",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            await _applicationService.Update(record.ApplicationId, updateReq);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationCacheKeys();

            var record = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

            // Act
            await _applicationService.Delete(record.ApplicationId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
