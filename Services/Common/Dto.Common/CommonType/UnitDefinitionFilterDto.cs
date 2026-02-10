namespace Dto.Common.CommonType
{
    public record UnitDefinitionFilterDto
    {
        public int UnitDefinitionId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UnitDefinitionName { get; set; }
    }
}
