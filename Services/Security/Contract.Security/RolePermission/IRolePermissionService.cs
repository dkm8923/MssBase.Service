using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Shared.Models;

namespace Contract.Security.RolePermission;

public interface IRolePermissionService
{
    public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseServiceGet req, bool includeRelated = false);
    public Task<ErrorValidationResult<RolePermissionDto>> GetById(int roleId, BaseServiceGet req, bool includeRelated = false);
    public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionServiceRequest req);
    public Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req);
    public Task<ErrorValidationResult<RolePermissionDto>> Update(int roleId, InsertUpdateRolePermissionRequest req);
    public Task<ErrorValidationResult> Delete(int roleId);
}
