using Contract.Security.Application;
using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly string cacheKeySectionName = ICacheService.PermissionService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IPermissionLogic _roleLogic;
        private readonly ICacheService _cacheService;

        public PermissionService(IApplicationLogic applicationLogic, IPermissionLogic roleLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _roleLogic = roleLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, permissionId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetById(permissionId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var permissionIdsKey = (req.PermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var nameKey = CacheUtilities.CreateKeyFromString(req.Name);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,permissionIdsKey
                ,nameKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Insert(req, _applicationLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Update(permissionId, req, _applicationLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int permissionId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Delete(permissionId);
        }

        #endregion
    }
}
