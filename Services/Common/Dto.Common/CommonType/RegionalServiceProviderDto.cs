namespace Dto.Common.CommonType
{
    public record RegionalServiceProviderDto
    {
        public string RegionalServiceProviderCode { get; set; }
        public bool Active { get; set; }
        public string RegionalServiceProviderName { get; set; }
    }
}
