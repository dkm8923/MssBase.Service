using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Shared.Models;
using Contract.Security.Application;

namespace Contract.Security.ApplicationUser
{
    public interface IApplicationUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserId);
        public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int applicationUserId);
        public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
    }
}
