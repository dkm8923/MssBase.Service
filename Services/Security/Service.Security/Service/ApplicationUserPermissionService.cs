using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.ApplicationUserPermission;
using Contract.Security.Permission;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class ApplicationUserPermissionService : IApplicationUserPermissionService
    {
        private readonly string cacheKeySectionName = ICacheService.ApplicationUserPermissionService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IApplicationUserLogic _applicationUserLogic;
        private readonly IPermissionLogic _permissionLogic;
        private readonly IApplicationUserPermissionLogic _applicationUserPermissionLogic;
        private readonly ICacheService _cacheService;

        public ApplicationUserPermissionService(IApplicationLogic applicationLogic,
                                                IApplicationUserLogic applicationUserLogic,
                                                IPermissionLogic permissionLogic,
                                                IApplicationUserPermissionLogic applicationUserPermissionLogic, 
                                                ICacheService cacheService
                                               )
        {
            _applicationLogic = applicationLogic;
            _applicationUserLogic = applicationUserLogic;
            _permissionLogic = permissionLogic;
            _applicationUserPermissionLogic = applicationUserPermissionLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.GetById(applicationUserId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationUserPermissionIdsKey = (req.ApplicationUserPermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var applicationIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationId);
            var applicationUserIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationUserId);
            var permissionIdKey = CacheUtilities.CreateKeyFromInt(req.PermissionId);
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationUserPermissionIdsKey
                ,applicationIdKey
                ,applicationUserIdKey
                ,permissionIdKey
                ,includeInactiveKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserPermissionLogic.Insert(req, _applicationLogic, _applicationUserLogic, _permissionLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserId, InsertUpdateApplicationUserPermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserPermissionLogic.Update(applicationUserId, req, _applicationLogic, _applicationUserLogic, _permissionLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int applicationUserId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserPermissionLogic.Delete(applicationUserId);
        }

        #endregion
    }
}
