using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
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
                                               IDefaultServiceTestsGetAllIncludeRelated,
                                               IDefaultServiceTestsGetAllReadOnly,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsGetByIdIncludeRelated,
                                               IDefaultServiceTestsGetByIdReadOnly,
                                               IDefaultServiceTestsGetAuditLogsById,
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

            var expectedCacheKey = $"ApplicationUserService_GetAll_0_0_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_1_0_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeReadOnly_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_0_0_1";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeReadOnly = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();
            var cacheKeyData = await _cacheTestUtilities.GetKeyData<List<ApplicationUserDto>>(expectedCacheKey);

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeRelated_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "ApplicationUserService_GetAll_0_1_0";

            // Act
            var result = await _applicationUserService.GetAll(new BaseServiceGet { IncludeRelated = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCount(1);
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
            var applicationUser = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetById_{applicationUser.ApplicationUserId}_0_0_0";

            // Act
            var result = await _applicationUserService.GetById(applicationUser.ApplicationUserId, new BaseServiceGet());
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
            var applicationUser = arrangeTestDataResponse.InactiveApplicationUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetById_{applicationUser.ApplicationUserId}_1_0_0";

            // Act
            var result = await _applicationUserService.GetById(applicationUser.ApplicationUserId, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeRelated_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeApplicationUserTestDataWithRelatedData();
            var applicationUser = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"ApplicationUserService_GetById_{applicationUser.ApplicationUserId}_0_1_0";

            // Act
            var result = await _applicationUserService.GetById(applicationUser.ApplicationUserId, new BaseServiceGet { IncludeRelated = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeReadOnly_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyApplicationUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = arrangeTestDataResponse.ActiveApplicationUsers.FirstOrDefault();
            var expectedCacheKey = $"ApplicationUserService_GetById_{testRecord.ApplicationUserId}_0_0_1";
    
            // Act
            var result = await _applicationUserService.GetById(testRecord.ApplicationUserId, new BaseServiceGet { IncludeReadOnly = true });
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

        #region GetAuditLogsById

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();
            
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var user = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId);

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                ApplicationId = testRecord.ApplicationId,
                UserId = testRecord.UserId,
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await _applicationUserLogic.Update(testRecord.ApplicationUserId, updateReq, _applicationLogic, _applicationUserLogic, _userLogic);
            var expectedCacheKey = $"ApplicationUserService_GetAuditLogById_{testRecord.ApplicationUserId}";

            var result = await _applicationUserService.GetAuditLogsByApplicationUserId(testRecord.ApplicationUserId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region Filter

        [Fact]
        public async Task Default_Filter_Should_Cache()
        {
            // Arrange
            var securityTestData = await ArrangeApplicationUserPermissionTestData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var user = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId);
            
            var readOnlyActiveApplication = (await _securityTestUtilities.Application.CreateActiveReadOnlyTestRecords(1)).First();
            var readOnlyActiveUser = (await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1)).First();
            await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecord(readOnlyActiveApplication.ApplicationId, readOnlyActiveUser.UserId);
            
            var readOnlyInactiveApplication = (await _securityTestUtilities.Application.CreateInactiveReadOnlyTestRecords(1)).First();
            var readOnlyInActiveUser = (await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1)).First();
            await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecord(readOnlyInactiveApplication.ApplicationId, readOnlyInActiveUser.UserId);

            await _cacheTestUtilities.DeleteAllKeyData();
           
            var insertReq = new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                UserId = user.UserId,
                CurrentUser = TestConstants.SpecificCurrentUserForInsert,
                Active = true
            };

           var applicationUserRes = await _applicationUserLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _userLogic);

           insertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;

           await _applicationUserLogic.Update(applicationUserRes.Response.ApplicationUserId, insertReq, _applicationLogic, _applicationUserLogic, _userLogic);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterApplicationUserServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert };
           var postReqCreatedOnDate = new FilterApplicationUserServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqUpdatedBy = new FilterApplicationUserServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate };
           var postReqUpdatedOnDate = new FilterApplicationUserServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow) };
           var postReqApplicationUserIds = new FilterApplicationUserServiceRequest { ApplicationUserIds = new List<int> { applicationUserRes.Response.ApplicationUserId } };
           var postReqApplicationId = new FilterApplicationUserServiceRequest { ApplicationId = application.ApplicationId };
           var postReqUserId = new FilterApplicationUserServiceRequest { UserId = user.UserId };
           var postReqIncludeInactive = new FilterApplicationUserServiceRequest { IncludeInactive = true };
           var postReqIncludeRelated = new FilterApplicationUserServiceRequest { IncludeRelated = true };
           var postReqIncludeReadOnly = new FilterApplicationUserServiceRequest { IncludeReadOnly = true };
           
           var expectedCacheKeyCreatedBy = $"ApplicationUserPermissionService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyCreatedOnDate = $"ApplicationUserPermissionService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy = $"ApplicationUserPermissionService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate = $"ApplicationUserPermissionService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyApplicationUserIds = $"ApplicationUserPermissionService_Filter_0_0_0_0_{(postReqApplicationUserIds.ApplicationUserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0_0_0";
           var expectedCacheKeyApplicationId = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqApplicationId.ApplicationId.ToString())}_0_0_0_0_0";
           var expectedCacheKeyUserId = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqUserId.UserId.ToString())}_0_0_0_0";
           var expectedCacheKeyIncludeInactive = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_0_0_1_0_0";
           var expectedCacheKeyIncludeRelated = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_0_0_0_1_0";
           var expectedCacheKeyIncludeReadOnly = $"ApplicationUserPermissionService_Filter_0_0_0_0_0_0_0_0_0_0_1";
           
           // Act
           var filterCreatedByResult = await _applicationUserService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _applicationUserService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _applicationUserService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _applicationUserService.Filter(postReqUpdatedOnDate);
           var filterApplicationUserIdsResult = await _applicationUserService.Filter(postReqApplicationUserIds);
           var filterApplicationIdResult = await _applicationUserService.Filter(postReqApplicationId);
           var filterUserIdResult = await _applicationUserService.Filter(postReqUserId);
           var filterIncludeInactiveResult = await _applicationUserService.Filter(postReqIncludeInactive);
           var filterIncludeRelatedResult = await _applicationUserService.Filter(postReqIncludeRelated);
           var filterIncludeReadOnlyResult = await _applicationUserService.Filter(postReqIncludeReadOnly);
           
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

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationUserIds);
           filterApplicationUserIdsResult.Response.Should().HaveCount(1);   

           availableCacheKeys.Should().Contain(expectedCacheKeyApplicationId);
           filterApplicationIdResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeyUserId);
           filterUserIdResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeInactive);
           filterIncludeInactiveResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeRelated);
           filterIncludeRelatedResult.Response.Should().HaveCountGreaterThan(0);

           availableCacheKeys.Should().Contain(expectedCacheKeyIncludeReadOnly);
           filterIncludeReadOnlyResult.Response.Should().HaveCountGreaterThan(0); 
        }

        #endregion

        #region Insert

        [Fact]
        public async Task Default_Insert_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var user = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserCacheKeys();

            var insertReq = new InsertUpdateApplicationUserRequest
            {
                ApplicationId = application.ApplicationId,
                UserId = user.UserId,
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

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
            var user = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId);
            
            var newApplication = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
            var newUser = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserCacheKeys();

            var updateReq = new InsertUpdateApplicationUserRequest
            {
                ApplicationId = newApplication.ApplicationId,
                UserId = newUser.UserId,
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
            var user = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            var testRecord = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId, user.UserId);
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateApplicationUserCacheKeys();

            // Act
            await _applicationUserService.Delete(testRecord.ApplicationUserId, TestConstants.CurrentUser);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
