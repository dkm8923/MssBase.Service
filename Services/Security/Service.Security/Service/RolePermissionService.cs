using Contract.Security.Application;
using Contract.Security.Permission;
using Contract.Security.Role;
using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
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

        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, rolePermissionId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.GetById(rolePermissionId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRolePermissionId(int rolePermissionId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, rolePermissionId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.GetAuditLogsByRolePermissionId(rolePermissionId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var rolePermissionIdsKey = (req.RolePermissionIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var applicationIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationId); 
            var roleIdKey = CacheUtilities.CreateKeyFromInt(req.RoleId); 
            var permissionIdKey = CacheUtilities.CreateKeyFromInt(req.PermissionId); 
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,rolePermissionIdsKey
                ,applicationIdKey
                ,roleIdKey
                ,permissionIdKey
                ,includeInactiveKey
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _rolePermissionLogic.Filter(req, cancellationToken));
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

        public async Task<ErrorValidationResult> Delete(int rolePermissionId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _rolePermissionLogic.Delete(rolePermissionId, currentUser);
        }

        #endregion
    }
}
