using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using Shared.Models;
using Contract.Security.Application;
using Dto.Security.Authentication;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUser
{
    public interface IApplicationUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserLogChangePasswordDto>>> GetPasswordChangeHistoryByApplicationUserId(int applicationUserId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser);
        public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int applicationUserId);
        public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
    }
}
