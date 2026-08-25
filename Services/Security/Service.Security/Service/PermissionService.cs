using Contract.Security.Application;
using Contract.Security.Permission;
using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly string cacheKeySectionName = ICacheService.PermissionService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IPermissionLogic _permissionLogic;
        private readonly ICacheService _cacheService;

        public PermissionService(IApplicationLogic applicationLogic, IPermissionLogic permissionLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _permissionLogic = permissionLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _permissionLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, permissionId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _permissionLogic.GetById(permissionId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(int permissionId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, permissionId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _permissionLogic.GetAuditLogsByPermissionId(permissionId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var permissionIdsKey = (req.PermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var nameKey = CacheUtilities.CreateKeyFromString(req.Name);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);
            
            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,permissionIdsKey
                ,nameKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _permissionLogic.Filter(req, cancellationToken));
        }

        #endregion
    
        #region Insert

        public async Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _permissionLogic.Insert(req, _applicationLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _permissionLogic.Update(permissionId, req, _applicationLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int permissionId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _permissionLogic.Delete(permissionId, currentUser);
        }

        #endregion
    }
}
