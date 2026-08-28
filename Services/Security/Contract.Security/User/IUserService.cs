using Dto.Security.User;
using Dto.Security.User.Service;
using Dto.Security.Authentication;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.User;

public interface IUserService
{
    public Task<ErrorValidationResult<IEnumerable<UserDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<UserDto>> GetById(int userId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserId(int userId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>>> GetPasswordChangeHistoryByUserId(int userId, bool deleteCache = false, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<UserDto>>> Filter(FilterUserServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<UserDto>> Insert(InsertUpdateUserRequest req);
    public Task<ErrorValidationResult<UserDto>> Update(int userId, InsertUpdateUserRequest req);
    public Task<ErrorValidationResult> Delete(int userId, string currentUser);
    public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int userId);
    public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
}
