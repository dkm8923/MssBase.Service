using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Role;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Shared.Models;

namespace Contract.Security.ApplicationUserRole
{
    public interface IApplicationUserRoleLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserRoleId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleLogicRequest req);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IRoleLogic roleLogic);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserRoleId, InsertUpdateApplicationUserRoleRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IRoleLogic roleLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserRoleId);
    }
}
