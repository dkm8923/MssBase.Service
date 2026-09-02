using Contract.Security.Application;
using Contract.Security.User;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUser
{
    public interface IApplicationUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IUserLogic userLogic);
        public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IUserLogic userLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser);
    }
}
