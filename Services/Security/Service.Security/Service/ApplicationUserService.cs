using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Dto.Security.Authentication;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly string cacheKeySectionName = ICacheService.ApplicationUserService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IApplicationUserLogic _applicationUserLogic;
        private readonly ICacheService _cacheService;

        public ApplicationUserService(IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _applicationUserLogic = applicationUserLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.GetById(applicationUserId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, applicationUserId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.GetAuditLogsByApplicationUserId(applicationUserId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserLogChangePasswordDto>>> GetPasswordChangeHistoryByApplicationUserId(int applicationUserId, bool deleteCache = false, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName + "_PasswordChangeHistory", applicationUserId);
            return await _cacheService.GetByKeyAsync(deleteCache, cacheKeyName, () => _applicationUserLogic.GetPasswordChangeHistoryByApplicationUserId(applicationUserId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationUserIdsKey = (req.ApplicationUserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var emailKey = CacheUtilities.CreateKeyFromString(req.Email);
            var firstNameKey = CacheUtilities.CreateKeyFromString(req.FirstName);
            var lastNameKey = CacheUtilities.CreateKeyFromString(req.LastName);
            //var dateOfBirthKey = CacheUtilities.CreateKeyFromString(req.DateOfBirth.ToString());
            var dateOfBirthKey = "0"; //TODO: Make this work, should be DateOnly
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);
            
            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationUserIdsKey
                ,emailKey
                ,firstNameKey
                ,lastNameKey
                ,dateOfBirthKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.Filter(req, cancellationToken));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserLogic.Insert(req, _applicationLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserLogic.Update(applicationUserId, req, _applicationLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserLogic.Delete(applicationUserId, currentUser);
        }

        #endregion

        public async Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int applicationUserId)
        {
            return await _applicationUserLogic.ResetPassword(applicationUserId);
        }

        public async Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req)
        {
            return await _applicationUserLogic.ChangePassword(req);
        }
    }
}
