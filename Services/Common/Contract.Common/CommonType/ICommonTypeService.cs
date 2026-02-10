using Dto.Common.CommonType;
using Dto.Common.CommonType.Service;
using Shared.Models;

namespace Contract.Common.CommonType
{
    public interface ICommonTypeService
    {
        public Task<ErrorValidationResult<IEnumerable<OriginSystemDto>>> GetAllOriginSystems(BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<ChargeCodeDto>>> GetAllChargeCodes(BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<FacilityDto>>> GetAllFacilities(BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<PackageIncentiveTierDto>>> GetAllPackageIncentiveTiers(BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<RegionalServiceProviderDto>>> GetAllRegionalServiceProviders(BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionFilterDto>>> GetAllUnitDefinitions(BaseServiceGet req);
        public Task<ErrorValidationResult<FilterCommonTypeResponseDto>> FilterCommonTypes(FilterCommonTypeServiceRequest req);
    }
}
