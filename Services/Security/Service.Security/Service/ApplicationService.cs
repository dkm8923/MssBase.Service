using Contract.Security.Application;
using Dto.Security.Application;
using Dto.Security.Application.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Security.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly string cacheKeySectionName = ICacheService.ApplicationService;
        private readonly IApplicationLogic _applicationLogic;
        private readonly ICacheService _cacheService;

        public ApplicationService(IApplicationLogic applicationLogic, ICacheService cacheService)
        {
            _applicationLogic = applicationLogic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<ApplicationDto>> GetById(int unitId, BaseServiceGet req)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, unitId, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationLogic.GetById(unitId, req));
        }

        public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var applicationIdsKey = (req.ApplicationIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var nameKey = CacheUtilities.CreateKeyFromString(req.Name);
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,applicationIdsKey
                ,nameKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationLogic.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationLogic.Insert(req);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<ApplicationDto>> Update(int ApplicationId, InsertUpdateApplicationRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationLogic.Update(ApplicationId, req);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int unitId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _applicationLogic.Delete(unitId);
        }

        #endregion
    }
}
