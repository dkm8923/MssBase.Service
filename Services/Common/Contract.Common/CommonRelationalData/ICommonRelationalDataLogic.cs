using Dto.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Logic;
using Shared.Models;

namespace Contract.Common.CommonRelationalData
{
    public interface ICommonRelationalDataLogic
    {
        public Task<ErrorValidationResult<FilterCommonRelationalDataDto>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<FilterCommonRelationalDataDto>> Filter(FilterCommonRelationalDataLogicRequest req, CancellationToken cancellationToken = default);
    }
}
