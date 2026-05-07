using Contract.Security.Application;
using Dto.Security.Permission;
using Dto.Security.Permission.Logic;
using Shared.Models;

namespace Contract.Security.Permission
{
    public interface IPermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<PermissionDto>> GetById(int permissionId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<PermissionDto>>> Filter(FilterPermissionLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<PermissionDto>> Insert(InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult<PermissionDto>> Update(int permissionId, InsertUpdatePermissionRequest req, IApplicationLogic applicationLogic);
        public Task<ErrorValidationResult> Delete(int permissionId);
    }
}
