using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.ApplicationUserPermission;
using Contract.Security.Permission;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
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

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.GetById(applicationUserId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserPermissionId(int applicationUserPermissionId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, applicationUserPermissionId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.GetAuditLogsByApplicationUserPermissionId(applicationUserPermissionId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionServiceRequest req, CancellationToken cancellationToken = default)
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
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);
            
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
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserPermissionLogic.Filter(req, cancellationToken));
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

        public async Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserPermissionLogic.Delete(applicationUserId, currentUser);
        }

        #endregion
    }
}
