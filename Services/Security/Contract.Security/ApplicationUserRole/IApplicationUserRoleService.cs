using System;
using Dto.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Service;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.ApplicationUserRole;

public interface IApplicationUserRoleService
{
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> GetAll(BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserRoleDto>> GetById(int applicationUserRoleId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<ApplicationUserRoleDto>>> Filter(FilterApplicationUserRoleServiceRequest req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByApplicationUserRoleId(int applicationUserRoleId, BaseServiceGet req, CancellationToken cancellationToken = default);
    public Task<ErrorValidationResult<ApplicationUserRoleDto>> Insert(InsertUpdateApplicationUserRoleRequest req);
    public Task<ErrorValidationResult<ApplicationUserRoleDto>> Update(int applicationUserRoleId, InsertUpdateApplicationUserRoleRequest req);
    public Task<ErrorValidationResult> Delete(int applicationUserRoleId, string currentUser);
}

