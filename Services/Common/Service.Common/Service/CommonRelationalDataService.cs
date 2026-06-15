using Contract.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Service;
using Shared.Contracts;
using Shared.Models;
using Shared.Service.Cache;

namespace Service.Common.Service
{
    public class CommonRelationalDataService : ICommonRelationalDataService
    {
        private readonly string cacheKeySectionName = ICacheService.CommonRelationalDataService;
        private readonly ICommonRelationalDataLogic _commonRelationalDataLogic;
        private readonly ICacheService _cacheService;

        public CommonRelationalDataService(ICommonRelationalDataLogic commonRelationalDataLogic, ICacheService cacheService)
        {
            _commonRelationalDataLogic = commonRelationalDataLogic;
            _cacheService = cacheService;
        }

        #region GET

        
        public async Task<ErrorValidationResult<FilterCommonRelationalDataDto>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default)
        {
            var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, req.IncludeRelated);
            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _commonRelationalDataLogic.GetAll(req));
        }

        public async Task<ErrorValidationResult<FilterCommonRelationalDataDto>> Filter(FilterCommonRelationalDataServiceRequest req, CancellationToken cancellationToken = default)
        {
            var createdByKey = CacheUtilities.CreateKeyFromString(req.CreatedBy);
            var createdOnKey = CacheUtilities.CreateKeyFromDateOnly(req.CreatedOnDate);
            var updatedByKey = CacheUtilities.CreateKeyFromString(req.UpdatedBy);
            var updatedOnKey = CacheUtilities.CreateKeyFromDateOnly(req.UpdatedOnDate);
            
            var includeInactiveKey = CacheUtilities.CreateKeyFromBool(req.IncludeInactive);

            // Normalize ReferenceTypes for consistent cache key generation. This includes trimming whitespace, converting to lower case, removing duplicates, sorting, and URL-encoding.
            var normalizedReferenceTypes = req.ReferenceTypes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLowerInvariant())
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .Select(Uri.EscapeDataString)
                .ToList();

            var referenceTypesKey = normalizedReferenceTypes.Count == 0 ? "0" : string.Join("|", normalizedReferenceTypes);

            var cacheKeyName = CacheUtilities.CreateFilterCacheKey(cacheKeySectionName, new List<string> {
                 createdByKey
                ,createdOnKey
                ,updatedByKey
                ,updatedOnKey
                ,includeInactiveKey
                ,referenceTypesKey
            });

            return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _commonRelationalDataLogic.Filter(req, cancellationToken));
        }

        #endregion
    }
}
