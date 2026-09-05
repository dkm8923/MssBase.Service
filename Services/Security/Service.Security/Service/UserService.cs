using Contract.Security.Application;
using Contract.Security.User;
using Dto.Security.User;
using Dto.Security.User.Service;
using Dto.Security.Authentication;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
using Shared.Service.Cache;
using Contract.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Service;
using Dto.Common.CommonRelationalData;
using static Shared.Logic.Common.Constants;

namespace Service.Security.Service
{
    public class UserService : IUserService
    {
        private readonly string cacheKeySectionName = ICacheService.UserService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly ICommonRelationalDataService _commonRelationalDataService;
        private readonly IUserLogic _userLogic;
        private readonly ICacheService _cacheService;

        public UserService(IApplicationLogic applicationLogic, 
                           IUserLogic applicationUserLogic, 
                           ICommonRelationalDataService commonRelationalDataService, 
                           ICacheService cacheService
                          )
        {
            _applicationLogic = applicationLogic;
            _userLogic = applicationUserLogic;
            _commonRelationalDataService = commonRelationalDataService;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<UserDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _userLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<UserDto>> GetById(int userId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, userId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _userLogic.GetById(userId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserId(int userId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, userId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _userLogic.GetAuditLogsByUserId(userId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>>> GetPasswordChangeHistoryByUserId(int userId, bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName + "_PasswordChangeHistory", userId);
            return await _cacheService.GetByKeyAsync(deleteCache, cacheKeyName, () => _userLogic.GetPasswordChangeHistoryByUserId(userId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<UserDto>>> Filter(FilterUserServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var userIdsKey = (req.UserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var emailKey = CacheUtilities.CreateKeyFromString(req.Email);
            var titleKey = CacheUtilities.CreateKeyFromString(req.Title);
            var firstNameKey = CacheUtilities.CreateKeyFromString(req.FirstName);
            var middleNameKey = CacheUtilities.CreateKeyFromString(req.MiddleName);
            var lastNameKey = CacheUtilities.CreateKeyFromString(req.LastName);
            var preferredNameKey = CacheUtilities.CreateKeyFromString(req.PreferredName);
            var suffixKey = CacheUtilities.CreateKeyFromString(req.Suffix);
            //var dateOfBirthKey = CacheUtilities.CreateKeyFromString(req.DateOfBirth.ToString());
            var dateOfBirthKey = "0"; //TODO: Make this work, should be DateOnly
            var timeZoneKey = CacheUtilities.CreateKeyFromString(req.TimeZone);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);
            
            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,userIdsKey
                ,emailKey
                ,titleKey
                ,firstNameKey
                ,middleNameKey
                ,lastNameKey
                ,preferredNameKey
                ,suffixKey
                ,dateOfBirthKey
                ,timeZoneKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _userLogic.Filter(req, cancellationToken));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<UserDto>> Insert(InsertUpdateUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);
            var commonData = await _getCommonRelationalDataForInsertUpdateValidation();
            return await _userLogic.Insert(req, commonData); 
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<UserDto>> Update(int userId, InsertUpdateUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);
            var commonData = await _getCommonRelationalDataForInsertUpdateValidation();
            return await _userLogic.Update(userId, req, commonData);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int userId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _userLogic.Delete(userId, currentUser);
        }

        #endregion

        public async Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int userId)
        {
            return await _userLogic.ResetPassword(userId);
        }

        public async Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req)
        {
            return await _userLogic.ChangePassword(req);
        }

        #region Private

        private async Task<FilterCommonRelationalDataDto> _getCommonRelationalDataForInsertUpdateValidation()
        {
            var commonDataRes = await _commonRelationalDataService.Filter(new FilterCommonRelationalDataServiceRequest
            {
                ReferenceTypes = new List<string>
                {
                    CommonRelationalDataReferenceTypes.PersonTitle,
                    CommonRelationalDataReferenceTypes.PersonSuffix,
                    CommonRelationalDataReferenceTypes.UsaTimeZone
                }
            });

            return commonDataRes.Response;
        }

        #endregion
    }
}
