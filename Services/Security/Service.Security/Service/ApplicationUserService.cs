using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Shared.Contracts;
using Shared.Models;
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

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.GetById(applicationUserId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationUserIdsKey = (req.ApplicationUserIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var emailKey = CacheUtilities.CreateKeyFromString(req.Email);
            var firstNameKey = CacheUtilities.CreateKeyFromString(req.FirstName);
            var lastNameKey = CacheUtilities.CreateKeyFromString(req.LastName);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationUserIdsKey
                ,emailKey
                ,firstNameKey
                ,lastNameKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserLogic.Filter(req));
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

        public async Task<ErrorValidationResult> Delete(int applicationUserId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserLogic.Delete(applicationUserId);
        }

        #endregion
    }
}
