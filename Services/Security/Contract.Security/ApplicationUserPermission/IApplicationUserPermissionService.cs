using System;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUserPermission;

public interface IApplicationUserPermissionService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserPermissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserPermissionId(int applicationUserPermissionId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserPermissionId, InsertUpdateApplicationUserPermissionRequest req);
    public Task<ErrorValidationResult> Delete(int applicationUserPermissionId, string currentUser);
}
