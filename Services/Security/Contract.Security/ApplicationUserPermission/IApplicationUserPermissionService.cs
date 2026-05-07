using System;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Service;
using Shared.Models;

namespace Contract.Security.ApplicationUserPermission;

public interface IApplicationUserPermissionService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> GetById(int applicationUserId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserPermissionDto>>> Filter(FilterApplicationUserPermissionServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Insert(InsertUpdateApplicationUserPermissionRequest req);
    public Task<ErrorValidationResult<ApplicationUserPermissionDto>> Update(int applicationUserId, InsertUpdateApplicationUserPermissionRequest req);
    public Task<ErrorValidationResult> Delete(int applicationUserPermissionId);
}
