using Contract.Security.Application;
using Contract.Security.Permission;
using Contract.Security.Role;
using Dto.Security.RolePermission;
using Dto.Security.RolePermission.Logic;
using Shared.Models;

namespace Contract.Security.RolePermission
{
    public interface IRolePermissionLogic
    {
        public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<RolePermissionDto>> GetById(int rolePermissionId, BaseLogicGet req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<IEnumerable<RolePermissionDto>>> Filter(FilterRolePermissionLogicRequest req, CancellationToken cancellationToken = default);
        public Task<ErrorValidationResult<RolePermissionDto>> Insert(InsertUpdateRolePermissionRequest req, 
                                                                     IApplicationLogic applicationLogic,
                                                                     IRoleLogic roleLogic,
                                                                     IPermissionLogic permissionLogic
                                                                    );
        public Task<ErrorValidationResult<RolePermissionDto>> Update(int rolePermissionId, 
                                                                     InsertUpdateRolePermissionRequest req, 
                                                                     IApplicationLogic applicationLogic,
                                                                     IRoleLogic roleLogic,
                                                                     IPermissionLogic permissionLogic
                                                                    );
        public Task<ErrorValidationResult> Delete(int rolePermissionId);
    }
}
