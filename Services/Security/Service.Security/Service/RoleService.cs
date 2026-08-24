using Contract.Security.Application;
using Contract.Security.Role;
using Dto.Security.Role;
using Dto.Security.Role.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Models.Dtos;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class RoleService : IRoleService
    {
        private readonly string cacheKeySectionName = ICacheService.RoleService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IRoleLogic _roleLogic;
        private readonly ICacheService _cacheService;

        public RoleService(IApplicationLogic applicationLogic, IRoleLogic roleLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _roleLogic = roleLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetAll(req, cancellationToken));
        }

        public async Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, roleId, req.IncludeInactive, req.IncludeRelated, req.IncludeReadOnly);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetById(roleId, req, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRoleId(int roleId, BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAuditLogByIdCacheKey(cacheKeySectionName, roleId);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetAuditLogsByRoleId(roleId, cancellationToken));
        }

        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var roleIdsKey = (req.RoleIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var nameKey = CacheUtilities.CreateKeyFromString(req.Name);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);
            var includeReadOnlyKey = CacheUtilities.CreateKeyFromBool(req.IncludeReadOnly);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,roleIdsKey
                ,nameKey
                ,applicationIdKey
                ,includeInactiveKey
                ,includeRelatedKey
                ,includeReadOnlyKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.Filter(req, cancellationToken));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Insert(req, _applicationLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Update(roleId, req, _applicationLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int roleId, string currentUser)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Delete(roleId, currentUser);
        }

        #endregion
    }
}
