using Dto.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Service;
using Shared.Models;

namespace Contract.Common.CommonRelationalData
{
    public interface ICommonRelationalDataService
    {
        public Task<ErrorValidationResult<FilterCommonRelationalDataDto>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<FilterCommonRelationalDataDto>> Filter(FilterCommonRelationalDataServiceRequest req, CancellationToken cancellationToken = default);
    }
}