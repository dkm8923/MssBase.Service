using Dto.Security.User;
using Dto.Security.User.Logic;
using Shared.Models;
using Dto.Security.Authentication;
using Shared.Models.Dtos;
using Dto.Common.CommonRelationalData;
using Shared.Models.Dtos.CommonNote;

namespace Contract.Security.User
{
    public interface IUserLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UserDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<UserDto>> GetById(int userId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserId(int userId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<UserLogChangePasswordDto>>> GetPasswordChangeHistoryByUserId(int userId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<UserDto>>> Filter(FilterUserLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<UserDto>> Insert(InsertUpdateUserRequest req, FilterCommonRelationalDataDto commonRelationalData);
        public Task<ErrorValidationResult<CommonNoteDto>> InsertNote(int userId, InsertUpdateCommonNoteRequest req, FilterCommonRelationalDataDto commonRelationalData);
        public Task<ErrorValidationResult<UserDto>> Update(int userId, InsertUpdateUserRequest req, FilterCommonRelationalDataDto commonRelationalData);
        public Task<ErrorValidationResult<CommonNoteDto>> UpdateNote(int userId, InsertUpdateCommonNoteRequest req, FilterCommonRelationalDataDto commonRelationalData);
        public Task<ErrorValidationResult> Delete(int userId, string currentUser);
        public Task<ErrorValidationResult> DeleteNote(int userId, int noteId, string currentUser);
        public Task<ErrorValidationResult<ResetPasswordResponse>> ResetPassword(int userId);
        public Task<ErrorValidationResult> ChangePassword(ChangePasswordRequest req);
    }
}
