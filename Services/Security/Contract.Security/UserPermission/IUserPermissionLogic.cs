using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Shared.Models;

namespace Contract.Security.ApplicationUserPermission
{
    public interface IApplicationUserPermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserPermissionId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionLogicRequest req);
        public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req);
        public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserPermissionId, InsertUpdateApplicationUserPermissionRequest req);
        public Task<ErrorValidationResult> Delete(int applicationUserPermissionId);
    }
}
