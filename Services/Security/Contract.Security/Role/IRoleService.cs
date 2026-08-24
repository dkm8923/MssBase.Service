using Dto.Security.Role;
using Dto.Security.Role.Service;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.Role;

public interface IRoleService
{
    public Task<ErrorValidationResult<IEnumerable<RoleDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<RoleDto>> GetById(int roleId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByRoleId(int roleId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<RoleDto>>> Filter(FilterRoleServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<RoleDto>> Insert(InsertUpdateRoleRequest req);
    public Task<ErrorValidationResult<RoleDto>> Update(int roleId, InsertUpdateRoleRequest req);
    public Task<ErrorValidationResult> Delete(int roleId, string currentUser);
}
