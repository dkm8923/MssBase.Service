using Contract.Security.User;
using Dto.Security.User;
using Dto.Security.User.Service;
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
    public class UserServiceTests : SecurityTestBase,
                                               IDefaultServiceTestsGetAll,
                                               IDefaultServiceTestsGetAllIncludeRelated,
                                               IDefaultServiceTestsGetAllReadOnly,
                                               IDefaultServiceTestsGetById,
                                               IDefaultServiceTestsGetByIdIncludeRelated,
                                               IDefaultServiceTestsGetByIdReadOnly,
                                               IDefaultServiceTestsGetAuditLogsById,
                                               //IDefaultServiceTestsFilter,
                                               IDefaultServiceTestsInsert,
                                               IDefaultServiceTestsUpdate,
                                               IDefaultServiceTestsDelete
    {
        private readonly IUserService _userService;
        private readonly ICacheTestUtilities _cacheTestUtilities;

        public UserServiceTests()
        {
            _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
            _userService = _serviceProvider.GetService<IUserService>();
        }

        #region utils

        private async Task CreateUserCacheKeys()
        {
            var result = await _userService.GetAll(new BaseServiceGet { DeleteCache = false, IncludeInactive = true });

            foreach (var record in result.Response)
            {
                await _userService.GetById(record.UserId, new BaseServiceGet());
                await _userService.Filter(new FilterUserServiceRequest { CreatedOnDate = DateOnly.FromDateTime(record.CreatedOn) });
                await _userService.Filter(new FilterUserServiceRequest { CreatedBy = record.CreatedBy });
                await _userService.Filter(new FilterUserServiceRequest { UpdatedOnDate = DateOnly.FromDateTime((DateTime)record.UpdatedOn) });
            }
        }

        private List<string> GetUserServiceCacheKeys()
        {
            return _cacheTestUtilities.GetKeys().Where(x => x.Contains("UserService")).ToList(); 
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task Default_GetAll_Active_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"UserService_GetAll_0_0_0";

            // Act
            var result = await _userService.GetAll(new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "UserService_GetAll_1_0_0";

            // Act
            var result = await _userService.GetAll(new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeReadOnly_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "UserService_GetAll_0_0_1";

            // Act
            var result = await _userService.GetAll(new BaseServiceGet { IncludeReadOnly = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();
            var cacheKeyData = await _cacheTestUtilities.GetKeyData<List<UserDto>>(expectedCacheKey);

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Default_GetAll_IncludeRelated_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = "UserService_GetAll_0_1_0";

            // Act
            var result = await _userService.GetAll(new BaseServiceGet { IncludeRelated = true });
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

            var expectedCacheKey = "UserService_GetAll_0";

            // Act
            var result = await _userService.GetAll(new BaseServiceGet());
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
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var applicationUser = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"UserService_GetById_{applicationUser.UserId}_0_0_0";

            // Act
            var result = await _userService.GetById(applicationUser.UserId, new BaseServiceGet());
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeInactive_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var user = arrangeTestDataResponse.InactiveUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"UserService_GetById_{user.UserId}_1_0_0";

            // Act
            var result = await _userService.GetById(user.UserId, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeRelated_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestDataWithRelatedData();
            var applicationUser = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();

            var expectedCacheKey = $"UserService_GetById_{applicationUser.UserId}_0_1_0";

            // Act
            var result = await _userService.GetById(applicationUser.UserId, new BaseServiceGet { IncludeRelated = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        [Fact]
        public async Task Default_GetById_IncludeReadOnly_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeReadOnlyUserTestData();
            await _cacheTestUtilities.DeleteAllKeyData();

            var testRecord = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            var expectedCacheKey = $"UserService_GetById_{testRecord.UserId}_0_0_1";
    
            // Act
            var result = await _userService.GetById(testRecord.UserId, new BaseServiceGet { IncludeReadOnly = true });
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
            var result = await _userService.GetById(id, new BaseServiceGet { IncludeInactive = true });
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            result.Response.Should().BeNull();
            availableCacheKeys.Should().HaveCount(0);
        }

        [Fact]
        public async Task PasswordChangeHistory_GetById_Should_Cache()
        {
            // Arrange
            var arrangeTestDataResponse = await ArrangeUserTestData();
            var user = arrangeTestDataResponse.ActiveUsers.FirstOrDefault();
            await _cacheTestUtilities.DeleteAllKeyData();
            var pswdChangeHistoryResponse = await ArrangeUserPasswordChangeHistoryTestData(user.UserId);

            var expectedCacheKey = $"UserService_PasswordChangeHistory_GetById_{user.UserId}_0_0";

            // Act
            var result = await _userService.GetPasswordChangeHistoryByUserId(user.UserId);
            var availableCacheKeys = _cacheTestUtilities.GetKeys();

            // Assert
            availableCacheKeys.Should().Contain(expectedCacheKey);
            result.Response.Should().NotBeNull();
        }

        #endregion

        #region GetAuditLogsById

        [Fact]
        public async Task Default_GetAuditLogsById_Should_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            await _cacheTestUtilities.DeleteAllKeyData();
            var commonData = await _securityTestUtilities.User.GetCommonRelationalDataForUserInsertUpdateValidation();
            
            var testRecord = await _securityTestUtilities.User.CreateSingleUserTestRecord();

            var updateReq = new InsertUpdateUserRequest
            {
                DateOfBirth = new DateOnly(2000, 1, 1),
                Email = "updated@test.com",
                FirstName = "Updated First Name",
                LastName = "Updated Last Name",
                Active = false,
                CurrentUser = TestConstants.CurrentUser
            };

            // Act
            var updateResult = await _userLogic.Update(testRecord.UserId, updateReq, commonData);
            var expectedCacheKey = $"UserService_GetAuditLogById_{testRecord.UserId}";

            var result = await _userService.GetAuditLogsByUserId(testRecord.UserId, new BaseServiceGet());
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
           await ClearAllSecurityTestTableData();
           await _securityTestUtilities.User.CreateActiveTestRecords();
           await _securityTestUtilities.User.CreateInactiveTestRecords();
           await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
           await _cacheTestUtilities.DeleteAllKeyData();

           var commonData = await _securityTestUtilities.User.GetCommonRelationalDataForUserInsertUpdateValidation();

           var userInsertReq = new InsertUpdateUserRequest
           {
               DateOfBirth = new DateOnly(2000, 1, 1),
               Email = "test@test.com",
               Title = "Ms.",
               FirstName = "Test First Name",
               MiddleName = "Test Middle Name",
               LastName = "Test Last Name",
               PreferredName = "Test Preferred Name",
               Suffix = "Sr.",
               TimeZone = "PST",
               Active = true,
               CurrentUser = TestConstants.SpecificCurrentUserForInsert
           };

           var userRes = await _userLogic.Insert(userInsertReq, commonData);

           userInsertReq.CurrentUser = TestConstants.SpecificCurrentUserForUpdate;
           userInsertReq.FirstName = "Updated First Name";

           await _userLogic.Update(userRes.Response.UserId, userInsertReq, commonData);

           await _cacheTestUtilities.DeleteAllKeyData();

           var postReqCreatedBy = new FilterUserServiceRequest { CreatedBy = TestConstants.SpecificCurrentUserForInsert, DeleteCache = true };
           var postReqCreatedOnDate = new FilterUserServiceRequest { CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow), DeleteCache = true };
           var postReqUpdatedBy = new FilterUserServiceRequest { UpdatedBy = TestConstants.SpecificCurrentUserForUpdate, DeleteCache = true };
           var postReqUpdatedOnDate = new FilterUserServiceRequest { UpdatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow), DeleteCache = true };
           var postReqUserIds = new FilterUserServiceRequest { UserIds = new List<int> { userRes.Response.UserId }, DeleteCache = true };
           var postReqEmail = new FilterUserServiceRequest { Email = userInsertReq.Email, DeleteCache = true };
           var postReqTitle = new FilterUserServiceRequest { Title = userInsertReq.Title, DeleteCache = true };
           var postReqFirstName = new FilterUserServiceRequest { FirstName = userInsertReq.FirstName, DeleteCache = true };
           var postReqMiddleName = new FilterUserServiceRequest { MiddleName = userInsertReq.MiddleName, DeleteCache = true };
           var postReqLastName = new FilterUserServiceRequest { LastName = userInsertReq.LastName, DeleteCache = true };
           var postReqPreferredName = new FilterUserServiceRequest { PreferredName = userInsertReq.PreferredName, DeleteCache = true };
           var postReqSuffix = new FilterUserServiceRequest { Suffix = userInsertReq.Suffix, DeleteCache = true };
           var postReqDateOfBirth = new FilterUserServiceRequest { DateOfBirth = new DateOnly(2000, 1, 1), DeleteCache = true };
           var postReqTimeZone = new FilterUserServiceRequest { TimeZone = userInsertReq.TimeZone, DeleteCache = true };
           var postReqMaritalStatus = new FilterUserServiceRequest { MaritalStatus = userInsertReq.MaritalStatus, DeleteCache = true };
           var postReqReligion = new FilterUserServiceRequest { Religion = userInsertReq.Religion, DeleteCache = true };
           var postReqGender = new FilterUserServiceRequest { Gender = userInsertReq.Gender, DeleteCache = true };

           var postReqIncludeInactive = new FilterUserServiceRequest { IncludeInactive = true, DeleteCache = true };
           var postReqIncludeRelated = new FilterUserServiceRequest { IncludeRelated = true, DeleteCache = true };
           var postReqIncludeReadOnly = new FilterUserServiceRequest { IncludeReadOnly = true, DeleteCache = true };
           
           //17 keys
           var expectedCacheKeyCreatedBy =        $"UserService_Filter_{postReqCreatedBy.CreatedBy}_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0"; 
           var expectedCacheKeyCreatedOnDate =    $"UserService_Filter_0_{postReqCreatedOnDate.CreatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedBy =        $"UserService_Filter_0_0_{postReqUpdatedBy.UpdatedBy}_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUpdatedOnDate =    $"UserService_Filter_0_0_0_{postReqUpdatedOnDate.UpdatedOnDate.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyUserIdsKey =       $"UserService_Filter_0_0_0_0_{(postReqUserIds.UserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString()}_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyEmail =            $"UserService_Filter_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqEmail.Email)}_0_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyTitle =            $"UserService_Filter_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqTitle.Title)}_0_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyFirstName =        $"UserService_Filter_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqFirstName.FirstName)}_0_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyMiddleName =       $"UserService_Filter_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqMiddleName.MiddleName)}_0_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyLastName =         $"UserService_Filter_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqLastName.LastName)}_0_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeyPreferredName =    $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqPreferredName.PreferredName)}_0_0_0_0_0_0_0_0_0";
           var expectedCacheKeySuffix =           $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqSuffix.Suffix)}_0_0_0_0_0_0_0_0";
           var expectedCacheKeyDateofBirth =      $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_{postReqDateOfBirth.DateOfBirth.Value.ToString("yyyy-MM-dd")}_0_0_0_0_0_0_0";
           var expectedCacheKeyTimeZone =         $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqTimeZone.TimeZone)}_0_0_0_0_0_0";
           var expectedCacheKeyMaritalStatus =    $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqMaritalStatus.MaritalStatus)}_0_0_0_0_0";
           var expectedCacheKeyReligion =         $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqReligion.Religion)}_0_0_0_0";
           var expectedCacheKeyGender =           $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_{CommonUtilities.RemoveWhiteSpaceFromString(postReqGender.Gender)}_0_0_0";
           var expectedCacheKeyIncludeInactive =  $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_1_0_0";
           var expectedCacheKeyIncludeRelated =   $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_1_0";
           var expectedCacheKeyIncludeReadOnly =  $"UserService_Filter_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_0_1";

           // Act
           var filterCreatedByResult = await _userService.Filter(postReqCreatedBy);
           var filterCreatedOnDateResult = await _userService.Filter(postReqCreatedOnDate);
           var filterUpdatedByResult = await _userService.Filter(postReqUpdatedBy);
           var filterUpdatedOnDateResult = await _userService.Filter(postReqUpdatedOnDate);
           var filterUserIdsResult = await _userService.Filter(postReqUserIds);
           var filterEmailResult = await _userService.Filter(postReqEmail);
           var filterTitleResult = await _userService.Filter(postReqTitle);
           var filterFirstNameResult = await _userService.Filter(postReqFirstName);
           var filterMiddleNameResult = await _userService.Filter(postReqMiddleName);
           var filterLastNameResult = await _userService.Filter(postReqLastName);
           var filterPreferredNameResult = await _userService.Filter(postReqPreferredName);
           var filterSuffixResult = await _userService.Filter(postReqSuffix);
           var filterDateOfBirthResult = await _userService.Filter(postReqDateOfBirth);
           var filterTimeZoneResult = await _userService.Filter(postReqTimeZone);
           var filterIncludeInactiveResult = await _userService.Filter(postReqIncludeInactive);
           var filterIncludeRelatedResult = await _userService.Filter(postReqIncludeRelated);
           var filterIncludeReadOnlyResult = await _userService.Filter(postReqIncludeReadOnly);
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

           availableCacheKeys.Should().Contain(expectedCacheKeyUserIdsKey);
           filterUserIdsResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyEmail);
           filterEmailResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyTitle);
           filterTitleResult.Response.Should().HaveCount(1);      
           
           availableCacheKeys.Should().Contain(expectedCacheKeyFirstName);
           filterFirstNameResult.Response.Should().HaveCount(1);   

           availableCacheKeys.Should().Contain(expectedCacheKeyMiddleName);
           filterMiddleNameResult.Response.Should().HaveCount(1);
           
           availableCacheKeys.Should().Contain(expectedCacheKeyLastName);
           filterLastNameResult.Response.Should().HaveCount(1);   
           
           availableCacheKeys.Should().Contain(expectedCacheKeySuffix);
           filterSuffixResult.Response.Should().HaveCount(1);   

           availableCacheKeys.Should().Contain(expectedCacheKeyDateofBirth);
           filterDateOfBirthResult.Response.Should().HaveCount(1);

           availableCacheKeys.Should().Contain(expectedCacheKeyTimeZone);
           filterTimeZoneResult.Response.Should().HaveCount(1);    

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
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateUserCacheKeys();

            var insertReq = _securityTestUtilities.User.CreateInsertUpdateRequestWithRandomValues();

            // Act
            var insertResult = await _userService.Insert(insertReq);
            var cacheKeysAfterInsert = GetUserServiceCacheKeys(); 

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
            var testRecord = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateUserCacheKeys();

            var updateReq = new InsertUpdateUserRequest
            {
                Email = "updated@test.com",
                FirstName = "First name Updated",
                LastName = "Last name Updated",
                CurrentUser = TestConstants.CurrentUser,
                Active = true
            };

            // Act
            var result = await _userService.Update(testRecord.UserId, updateReq);
            var cacheKeysAfterDelete = GetUserServiceCacheKeys();

            // Assert
            result.Errors.Should().BeNullOrEmpty();
            cacheKeysAfterDelete.Should().HaveCount(0);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Default_Delete_Should_Delete_Cache()
        {
            // Arrange
            await ClearAllSecurityTestTableData();
            var testRecord = await _securityTestUtilities.User.CreateSingleUserTestRecord();
            await _cacheTestUtilities.DeleteAllKeyData();
            await CreateUserCacheKeys();

            // Act
            await _userService.Delete(testRecord.UserId, TestConstants.CurrentUser);
            var availableCacheKeys = GetUserServiceCacheKeys();

            //Assert
            availableCacheKeys.Should().HaveCount(0);
        }

        #endregion
    }
}
