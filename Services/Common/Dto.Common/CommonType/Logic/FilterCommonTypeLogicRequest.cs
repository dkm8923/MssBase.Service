using Shared.Models;

namespace Dto.Common.CommonType.Logic
{
    public record FilterCommonTypeLogicRequest : BaseLogicGet
    {
        public bool OriginSystems { get; set; }
        public bool ChargeCodes { get; set; }
        public bool Facilities { get; set; }
        public bool PackageIncentiveTiers { get; set; }
        public bool RegionalServiceProviders { get; set; }
        public bool UnitDefinitions { get; set; }
    }
}
