using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Service;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.RolePermission;

public interface IRolePermissionService
{
    public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRolePermissionId(int rolePermissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req);
    public Task<ErrorValidationResult<RolePermissionDto>> Update(int rolePermissionId, InsertUpdateRolePermissionRequest req);
    public Task<ErrorValidationResult> Delete(int rolePermissionId, string currentUser);
}
