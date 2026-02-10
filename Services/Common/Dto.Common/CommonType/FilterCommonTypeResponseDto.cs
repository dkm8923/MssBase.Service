namespace Dto.Common.CommonType
{
    public record FilterCommonTypeResponseDto
    {
        public FilterCommonTypeResponseDto() 
        {
            this.OriginSystems = new List<OriginSystemDto>();
            this.ChargeCodes = new List<ChargeCodeDto>();
            this.Facilities = new List<FacilityDto>();
            this.PackageIncentiveTiers = new List<PackageIncentiveTierDto>();
            this.RegionalServiceProviders = new List<RegionalServiceProviderDto>();
            this.UnitDefinitions = new List<UnitDefinitionFilterDto>();
        }

        public List<OriginSystemDto> OriginSystems { get; set; }
        public List<ChargeCodeDto> ChargeCodes { get; set; }
        public List<FacilityDto> Facilities { get; set; }
        public List<PackageIncentiveTierDto> PackageIncentiveTiers { get; set; }
        public List<RegionalServiceProviderDto> RegionalServiceProviders { get; set; }
        public List<UnitDefinitionFilterDto> UnitDefinitions { get; set; }
    }
}
