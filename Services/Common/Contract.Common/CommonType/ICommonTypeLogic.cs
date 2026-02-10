using Dto.Common.CommonType;
using Dto.Common.CommonType.Logic;
using Shared.Models;

namespace Contract.Common.CommonType
{
    public interface ICommonTypeLogic
    {
        public Task<ErrorValidationResult<IEnumerable<OriginSystemDto>>> GetAllOriginSystems(BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<ChargeCodeDto>>> GetAllChargeCodes(BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<FacilityDto>>> GetAllFacilities(BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<PackageIncentiveTierDto>>> GetAllPackageIncentiveTiers(BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<RegionalServiceProviderDto>>> GetAllRegionalServiceProviders(BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<UnitDefinitionFilterDto>>> GetAllUnitDefinitions(BaseLogicGet req, ICommonLogicManager commonLogicManager);
        public Task<ErrorValidationResult<FilterCommonTypeResponseDto>> FilterCommonTypes(FilterCommonTypeLogicRequest req, ICommonLogicManager commonLogicManager);
    }
}
