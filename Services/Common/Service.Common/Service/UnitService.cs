using Contract.Common;
using Contract.Common.Unit;
using Dto.Common.Unit;
using Dto.Common.Unit.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Common.Service
{
    public class UnitService : IUnitService
    {
        private readonly string cacheKeySectionName = ICacheService.UnitService;

        private readonly ICommonLogicManager _logic;
        private readonly ICacheService _cacheService;

        public UnitService(ICommonLogicManager logic, ICacheService cacheService)
        {
            _logic = logic;
            _cacheService = cacheService;
        }

        #region GET

        public async Task<ErrorValidationResult<IEnumerable<UnitDto>>> GetAll(BaseServiceGet req, bool includeRelated = false)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, includeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _logic.Unit.GetAll(req, includeRelated));
        }

        public async Task<ErrorValidationResult<UnitDto>> GetById(int unitId, BaseServiceGet req, bool includeRelated = false)
        {
            var cacheKeyName = CacheUtilities.CreateGetByIdCacheKey(cacheKeySectionName, unitId, req.IncludeInactive, includeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _logic.Unit.GetById(unitId, req, includeRelated));
        }

        public async Task<ErrorValidationResult<IEnumerable<UnitDto>>> Filter(FilterUnitServiceRequest req)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            var unitIdsKey = (req.UnitIds?.ConvertAll(Convert.ToInt32).Sum() ?? 0).ToString();
            var unitCodeKey = CacheUtilities.CreateKeyFromString(req.UnitCode);
            var unitNameKey = CacheUtilities.CreateKeyFromString(req.UnitName);
            var originSystemKey = CacheUtilities.CreateKeyFromString(req.OriginSystem);
            var unitDefinitionIdUnitQtyKey = CacheUtilities.CreateKeyFromInt(req.UnitDefinitionIdUnitQty);
            var unitDefinitionIdUnitValueKey = CacheUtilities.CreateKeyFromInt(req.UnitDefinitionIdUnitValue);
            var valueTypeNameKey = CacheUtilities.CreateKeyFromString(req.ValueTypeName);
            var unitPrepQueryKey = CacheUtilities.CreateKeyFromString(req.UnitPrepQuery);
            var unitHeaderQueryKey = CacheUtilities.CreateKeyFromString(req.UnitHeaderQuery);
            var unitUpdateQueryKey = CacheUtilities.CreateKeyFromString(req.UnitUpdateQuery);
            var chargeCodeKey = CacheUtilities.CreateKeyFromString(req.ChargeCode);
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);
            var includeRelatedKey = CacheUtilities.CreateKeyFromBool(req.IncludeRelated);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,unitIdsKey
                ,unitCodeKey
                ,unitNameKey
                ,originSystemKey
                ,unitDefinitionIdUnitQtyKey
                ,unitDefinitionIdUnitValueKey
                ,valueTypeNameKey
                ,unitPrepQueryKey
                ,unitHeaderQueryKey
                ,unitUpdateQueryKey
                ,chargeCodeKey
                ,includeInactiveKey
                ,includeRelatedKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _logic.Unit.Filter(req));
        }

        #endregion

        #region Insert

        public async Task<ErrorValidationResult<UnitDto>> Insert(InsertUpdateUnitRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _logic.Unit.Insert(req);
        }

        #endregion

        #region Update

        public async Task<ErrorValidationResult<UnitDto>> Update(int UnitId, InsertUpdateUnitRequest req)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _logic.Unit.Update(UnitId, req);
        }

        #endregion

        #region Delete

        public async Task<ErrorValidationResult> Delete(int unitId)
        {
            await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);

            return await _logic.Unit.Delete(unitId);
        }

        #endregion
    }
}
