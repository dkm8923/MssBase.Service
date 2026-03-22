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
                                               IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
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
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { CreatedBy = record.CreatedBy });
                await _applicationUserService.Filter(new FilterApplicationUserServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_1_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeInactive = true });
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
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var aplicationUser = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetById_{aplicationUser.ApplicationUserId}_0_0";

            // Act
            var result = await _applicationUserService.GetById(aplicationUser.ApplicationUserId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            var aplicationUser = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetById_{aplicationUser.ApplicationUserId}_1_0";

            // Act
            var result = await _applicationUserService.GetById(aplicationUser.ApplicationUserId, new BaseServiceGet { IncludeInactive = true });
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
           await ClearAllSecurityTestTableData();
           var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
           await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);
           await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId);

           var applicationUserInsertReq = new InsertUpdateApplicationUserRequest
           {
               ApplicationId = application.ApplicationId,
               DateOfBirth = DateTime.Parse("01/01/2000"),
               Email = "test@test.com",
               FirstName = "Test First Name",
               LastName = "Test Last Name",
               Active = true,
               CurrentUser = TestConstants.SpecificCurrentUserForInsert
           };

           var applicationUserRes = await _applicationUserLogic.Insert(applicationUserInsertReq, _applicationLogic);

           applicationUserInsertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _applicationUserLogic.Update(applicationUserRes.Response.ApplicationUserId, applicationUserInsertReq, _applicationLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterApplicationUserServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterApplicationUserServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterApplicationUserServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterApplicationUserServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqApplicationUserIds = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUserRes.Response.ApplicationUserId } };
           var postReqEmail = new FilterApplicationUserServiceRequest { Email = "test@test.com" };
           var postReqFirstName = new FilterApplicationUserServiceRequest { FirstName = "Test First Name" };
           var postReqLastName = new FilterApplicationUserServiceRequest { LastName = "Test Last Name" };
           var postReqDateOfBirth = new FilterApplicationUserServiceRequest { DateOfBirth = DateTime.Parse("01/01/2000") };
           var postReqApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = application.ApplicationId };
           var postReqIncludeInactive = new FilterApplicationUserServiceRequest { IncludeInactive = true };
           var postReqIncludeRelated = new FilterApplicationUserServiceRequest { IncludeRelated = true };
           
           var expectedCacheKeyCreatedBy = $"ApplicationUserService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"ApplicationUserService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"ApplicationUserService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"ApplicationUserService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyApplicationUserIdsKey = $"ApplicationUserService_Filter_0_0_0_0_{(postReqApplicationUserIds.ApplicationUserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0_0_0_0";
           var expectedCacheKeyEmail = $"ApplicationUserService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqEmail.Email)}_0_0_0_0_0_0";
           var expectedCacheKeyFirstName = $"ApplicationUserService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqFirstName.FirstName)}_0_0_0_0_0";
           var expectedCacheKeyLastName = $"ApplicationUserService_Filter_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqLastName.LastName)}_0_0_0_0";
           var expectedCacheKeyDateofBirth = $"ApplicationUserService_Filter_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqDateOfBirth.DateOfBirth.ToString())}_0_0_0";
           var expectedCacheKeyApplicationId = $"ApplicationUserService_Filter_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0";
           var expectedCacheKeyIncludeInactive = $"ApplicationUserService_Filter_0_0_0_0_0_0_0_0_0_0_1_0";
           var expectedCacheKeyIncludeRelated = $"ApplicationUserService_Filter_0_0_0_0_0_0_0_0_0_0_0_1";

           // Act
           var filterCreatedByResult = await _applicationUserService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _applicationUserService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _applicationUserService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _applicationUserService.Filter(postReqUpdatedOnDate);
           var filterApplicationUserIdsResult = await _applicationUserService.Filter(postReqApplicationUserIds);
           var filterEmailResult = await _applicationUserService.Filter(postReqEmail);
           var filterFirstNameResult = await _applicationUserService.Filter(postReqFirstName);
           var filterLastNameResult = await _applicationUserService.Filter(postReqLastName);
           var filterApplicationIdResult = await _applicationUserService.Filter(postReqApplicationId);
           var filterIncludeInactiveResult = await _applicationUserService.Filter(postReqIncludeInactive);
           var filterIncludeRelatedResult = await _applicationUserService.Filter(postReqIncludeRelated);
           
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

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserIdsKey);
           filterApplicationUserIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyEmail);
           filterEmailResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyFirstName);
           filterFirstNameResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyLastName);
           filterLastNameResult.Response.Should().HaveCount(1);   
           
           //TODO: Revisit this
        //    availableCacheKeys.Should().Contain(expectedCacheKeyDateofBirth);
        //    filterDateOfBirthResult.Response.Should().HaveCount(1);   

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
            await CreateApplicationUserCacheKeys();

            var insertReq = _securityTestUtilities.ApplicationUser.CreateInsertUpdateRequestWithRandomValues(application.ApplicationId);

            // Act
            var insertResult = await _applicationUserService.Insert(insertReq);
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
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserCacheKeys();

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                Email = "updated@test.com",
                FirstName = "First name Updated",
                LastName = "Last name Updated",
                ApplicationId = application.ApplicationId,
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
        public async Task Default_Delete_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserCacheKeys();

            // Act
            await _applicationUserService.Delete(testRecord.ApplicationUserId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
