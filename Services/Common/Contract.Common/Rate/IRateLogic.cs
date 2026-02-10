using Dto.Common.Rate;
using Dto.Common.Rate.Logic;
using Shared.Models;

namespace Contract.Common.Rate
{
    public interface IRateLogic
    {
        public Task<ErrorValidationResult<IEnumerable<RateDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<RateDto>> GetById(long rateId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<RateDto>>> Filter(FilterRateLogicRequest req);
        public Task<ErrorValidationResult<RateDto>> Insert(InsertUpdateRateRequest req);
        public Task<ErrorValidationResult<RateDto>> Update(long rateId, InsertUpdateRateRequest req);
        public Task<ErrorValidationResult> Delete(long rateId);
    }
}
