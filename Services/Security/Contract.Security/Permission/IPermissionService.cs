using Dto.Security.Permission;
using Dto.Security.Permission.Service;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.Permission;

public interface IPermissionService
{
    public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(int permissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req);
    public Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req);
    public Task<ErrorValidationResult> Delete(int permissionId, string currentUser);
}
