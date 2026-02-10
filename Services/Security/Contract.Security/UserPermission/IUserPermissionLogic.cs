using Dto.Security.UserPermission;
using Dto.Security.UserPermission.Logic;
using Shared.Models;

namespace Contract.Security.UserPermission
{
    public interface IUserPermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<UserPermissionDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<UserPermissionDto>> GetById(int userPermissionId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<UserPermissionDto>>> Filter(FilterUserPermissionLogicRequest req);
        public Task<ErrorValidationResult<UserPermissionDto>> Insert(InsertUpdateUserPermissionRequest req);
        public Task<ErrorValidationResult<UserPermissionDto>> Update(int userPermissionId, InsertUpdateUserPermissionRequest req);
        public Task<ErrorValidationResult> Delete(int userPermissionId);
    }
}
