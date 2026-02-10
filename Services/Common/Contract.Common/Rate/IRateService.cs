using Dto.Common.Rate;
using Dto.Common.Rate.Service;
using Shared.Models;

namespace Contract.Common.Rate
{
    public interface IRateService
    {
        public Task<ErrorValidationResult<IEnumerable<RateDto>>> GetAll(BaseServiceGet req);
        public Task<ErrorValidationResult<RateDto>> GetById(long rateId, BaseServiceGet req);
        public Task<ErrorValidationResult<IEnumerable<RateDto>>> Filter(FilterRateServiceRequest req);
        public Task<ErrorValidationResult<RateDto>> Insert(InsertUpdateRateRequest req);
        public Task<ErrorValidationResult<RateDto>> Update(long rateId, InsertUpdateRateRequest req);
        public Task<ErrorValidationResult> Delete(long rateId);
    }
}
