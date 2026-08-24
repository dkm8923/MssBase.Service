using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Role;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUserRole
{
    public interface IApplicationUserRoleLogic
    {
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserRoleId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserRoleId(int applicationUserRoleId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IRoleLogic roleLogic);
        public Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserRoleId, InsertUpdateApplicationUserRoleRequest req, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IRoleLogic roleLogic);
        public Task<ErrorValidationResult> Delete(int applicationUserRoleId, string currentUser);
    }
}
