using Dto.Security.Role;
using Dto.Security.Role.Service;
using Shared.Models;

namespace Contract.Security.Role;

public interface IRoleService
{
    public Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseServiceGet req);
    public Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseServiceGet req);
    public Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleServiceRequest req);
    public Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req);
    public Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req);
    public Task<ErrorValidationResult> Delete(int roleId);
}
