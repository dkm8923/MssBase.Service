using Shared.Models.Contracts;
using Dto.Common.CommonRelationalData.Logic;

namespace Dto.Common.CommonRelationalData.Service
{
    public record FilterCommonRelationalDataServiceRequest : FilterCommonRelationalDataLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}