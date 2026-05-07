using Contract.Security.Application;
using Dto.Security.Role;
using Dto.Security.Role.Logic;
using Shared.Models;

namespace Contract.Security.Role
{
    public interface IRoleLogic
    {
        public Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult> Delete(int roleId);
    }
}
