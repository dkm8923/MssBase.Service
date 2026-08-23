using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Service;
using Dto.Security.Authentication;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUser;

public interface IApplicationUserService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserDto>> GetById(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserId(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserLogChangePasswordDto>>> GetPasswordChangeHistoryByApplicationUserId(int applicationUserId, bool deleteCache = false, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserDto>>> Filter(FilterApplicationUserServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserDto>> Insert(InsertUpdateApplicationUserRequest req);
    public Task<ErrorValidationResult<ApplicationUserDto>> Update(int applicationUserId, InsertUpdateApplicationUserRequest req);
    public Task<ErrorValidationResult> Delete(int applicationUserId, string currentUser);
    public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int applicationUserId);
    public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
}
