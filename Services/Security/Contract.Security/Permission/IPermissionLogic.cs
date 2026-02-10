using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using Shared.Models;

namespace Contract.Security.Permission
{
    public interface IPermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionLogicRequest req);
        public Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req);
        public Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req);
        public Task<ErrorValidationResult> Delete(int permissionId);
    }
}
