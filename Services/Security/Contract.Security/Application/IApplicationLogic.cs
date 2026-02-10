using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Shared.Models;

namespace Contract.Security.Application
{
    public interface IApplicationLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationLogicRequest req);
        public Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req);
        public Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req);
        public Task<ErrorValidationResult> Delete(int applicationId);
    }
}
