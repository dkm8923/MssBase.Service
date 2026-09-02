using Dto.Security.User;
using Dto.Security.User.Logic;
using Shared.Models;
using Contract.Security.Application;
using Dto.Security.Authentication;
using Shared.Models.Dtos;

namespace Contract.Security.User
{
    public interface IUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<UserDto>> GetById(int userId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserId(int userId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>>> GetPasswordChangeHistoryByUserId(int userId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<UserDto>>> Filter(FilterUserLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<UserDto>> Insert(InsertUpdateUserRequest req);
        public Task<ErrorValidationResult<UserDto>> Update(int userId, InsertUpdateUserRequest req);
        public Task<ErrorValidationResult> Delete(int userId, string currentUser);
        public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int userId);
        public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
    }
}
