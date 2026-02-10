using Shared.Models.Contracts;

namespace Dto.Common.UnitDefinition
{
    public record InsertUpdateUnitDefinitionRequest : ICurrentUser
    {
        public bool Active { get; set; }
        public string OriginSystem { get; set; }
        public string SourceColumn { get; set; }
        public string DestinationColumn { get; set; }
        public string UserFriendlyDescription { get; set; }
        public bool UnitValue { get; set; }
        public string UnitValueColumn { get; set; }
        public bool UnitQty { get; set; }
        public string UnitQtyColumn { get; set; }
        public bool UnitQuery { get; set; }
        public string UnitQueryColumn { get; set; }
        public short UnitQueryPosition { get; set; }
        public bool GroupBy { get; set; }
        public string GroupByColumn { get; set; }
        public bool SupplementalGroupBy { get; set; }
        public string SupplementalGroupByColumn { get; set; }
        public bool PkgQty { get; set; }
        public string PkgQtyColumn { get; set; }
        public bool ConditionalAdjustment { get; set; }
        public string ConditionalAdjustmentColumn { get; set; }
        public string SqlDataType { get; set; }
        public string ListObjectName { get; set; }
        public bool UseList { get; set; }
        public bool UsePrimaryKey { get; set; }
        public bool EvaluateAsString { get; set; }
        public bool ExtraCriteria { get; set; }
        public string CurrentUser { get; set; }
    }
}
