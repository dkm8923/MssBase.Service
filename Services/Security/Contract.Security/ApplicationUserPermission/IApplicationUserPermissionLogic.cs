using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Permission;
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
        public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IPermissionLogic permissionLogic);
        public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserPermissionId, InsertUpdateApplicationUserPermissionRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IPermissionLogic permissionLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserPermissionId);
    }
}
