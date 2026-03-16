using Contract.Security.Application;
using Contract.Security.Permission;
using Contract.Security.Role;
using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly string cacheKeySectionName = ICacheService.RolePermissionService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IRolePermissionLogic _rolePermissionLogic;
        private readonly IRoleLogic _roleLogic;
        private readonly IPermissionLogic _permissionLogic;
        private readonly ICacheService _cacheService;

        public RolePermissionService(IApplicationLogic applicationLogic, IRolePermissionLogic rolePermissionLogic, IRoleLogic roleLogic, IPermissionLogic permissionLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _rolePermissionLogic = rolePermissionLogic;
            _roleLogic = roleLogic;
            _permissionLogic = permissionLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, rolePermissionId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.GetById(rolePermissionId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var rolePermissionIdsKey = (req.RolePermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var applicationIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationId); 
            var permissionIdKey = CacheUtilities.CreateKeyFromInt(req.PermissionId); 
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,rolePermissionIdsKey
                ,applicationIdKey
                ,permissionIdKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _rolePermissionLogic.Insert(req, _applicationLogic, _roleLogic, _permissionLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<RolePermissionDto>> Update(int rolePermissionId, InsertUpdateRolePermissionRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _rolePermissionLogic.Update(rolePermissionId, req, _applicationLogic, _roleLogic, _permissionLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int rolePermissionId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _rolePermissionLogic.Delete(rolePermissionId);
        }

        #endregion
    }
}
