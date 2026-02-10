using Dto.Security.Role;
using Dto.Security.Role.Logic;
using Shared.Models;

namespace Contract.Security.Role
{
    public interface IRoleLogic
    {
        public Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseLogicGet req);
        public Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseLogicGet req);
        public Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleLogicRequest req);
        public Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req);
        public Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req);
        public Task<ErrorValidationResult> Delete(int roleId);
    }
}
