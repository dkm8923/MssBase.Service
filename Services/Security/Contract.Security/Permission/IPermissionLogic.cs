using Contract.Security.Application;
using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using Shared.Models;
using Shared.Models.Dtos;

namespace Contract.Security.Permission
{
    public interface IPermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<AuditLogDto>>> GetAuditLogsByPermissionId(int permissionId, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult> Delete(int permissionId, string currentUser);
    }
}
