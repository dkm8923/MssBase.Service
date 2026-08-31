using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.User;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
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
        private readonly IUserLogic _userLogic;
        private readonly IApplicationUserLogic _applicationUserUserLogic;
        private readonly ICacheService _cacheService;

        public ApplicationUserService(IApplicationLogic applicationLogic,
                                                IApplicationUserLogic applicationUserLogic,
                                                IUserLogic userLogic,
                                                IApplicationUserLogic applicationUserUserLogic, 
                                                ICacheService cacheService
                                               )
        {
            _applicationLogic = applicationLogic;
            _applicationUserLogic = applicationUserLogic;
            _userLogic = userLogic;
            _applicationUserUserLogic = applicationUserUserLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserUserLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserUserLogic.GetById(applicationUserId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserUserId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, applicationUserUserId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserUserLogic.GetAuditLogsByApplicationUserId(applicationUserUserId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationUserUserIdsKey = (req.ApplicationUserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var applicationIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationId);
            var userIdKey = CacheUtilities.CreateKeyFromInt(req.UserId);
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);
            
            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationUserUserIdsKey
                ,applicationIdKey
                ,userIdKey
                ,includeInactiveKey
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserUserLogic.Filter(req, cancellationToken));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserUserLogic.Insert(req, _applicationLogic, _applicationUserLogic, _userLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserUserLogic.Update(applicationUserId, req, _applicationLogic, _applicationUserLogic, _userLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserUserLogic.Delete(applicationUserId, currentUser);
        }

        #endregion
    }
}
