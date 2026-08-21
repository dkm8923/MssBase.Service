using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.Application
{
    public interface IApplicationLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationId(int applicationId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req);
        public Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req);
        public Task<ErrorValidationResult<ApplicationDto>> Delete(int applicationId, string currentUser);
    }
}
