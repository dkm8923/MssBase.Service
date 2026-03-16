using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.ApplicationUserRole;
using Contract.Security.Role;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class ApplicationUserRoleService : IApplicationUserRoleService
    {
        private readonly string cacheKeySectionName = ICacheService.ApplicationUserRoleService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly IApplicationUserLogic _applicationUserLogic;
        private readonly IRoleLogic _roleLogic;
        private readonly IApplicationUserRoleLogic _applicationUserRoleLogic;
        private readonly ICacheService _cacheService;

        public ApplicationUserRoleService(IApplicationLogic applicationLogic,
                                                IApplicationUserLogic applicationUserLogic,
                                                IRoleLogic roleLogic,
                                                IApplicationUserRoleLogic applicationUserRoleLogic, 
                                                ICacheService cacheService
                                               )
        {
            _applicationLogic = applicationLogic;
            _applicationUserLogic = applicationUserLogic;
            _roleLogic = roleLogic;
            _applicationUserRoleLogic = applicationUserRoleLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserRoleLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, applicationUserId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserRoleLogic.GetById(applicationUserId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationUserRoleIdsKey = (req.ApplicationUserRoleIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var applicationIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationId);
            var applicationUserIdKey = CacheUtilities.CreateKeyFromInt(req.ApplicationUserId);
            var roleIdKey = CacheUtilities.CreateKeyFromInt(req.RoleId);
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationUserRoleIdsKey
                ,applicationIdKey
                ,applicationUserIdKey
                ,roleIdKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationUserRoleLogic.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserRoleLogic.Insert(req, _applicationLogic, _applicationUserLogic, _roleLogic);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserId, InsertUpdateApplicationUserRoleRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserRoleLogic.Update(applicationUserId, req, _applicationLogic, _applicationUserLogic, _roleLogic);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int applicationUserId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationUserRoleLogic.Delete(applicationUserId);
        }

        #endregion
    }
}
