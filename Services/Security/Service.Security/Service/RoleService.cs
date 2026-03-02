using Contract.Security.Application;
using Contract.Security.Role;
using Dto.Security.Role;
using Dto.Security.Role.Service;
using Shared.Contracts;
using Shared.Models;
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

        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseServiceGet req, bool includeRelated = false)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, includeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseServiceGet req, bool includeRelated = false)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, roleId, req.IncludeInactive, includeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.GetById(roleId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var roleIdsKey = (req.RoleIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var nameKey = CacheUtilities.CreateKeyFromString(req.Name);
            var applicationIdKey = (req.ApplicationId ?? 0).ToString();
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,roleIdsKey
                ,nameKey
                ,applicationIdKey
                ,includeInactiveKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _roleLogic.Filter(req));
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

        public async Task<ErrorValidationResult> Delete(int roleId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _roleLogic.Delete(roleId);
        }

        #endregion
    }
}
