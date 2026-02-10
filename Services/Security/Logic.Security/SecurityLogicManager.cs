using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Permission;
using Contract.Security.Role;
using Contract.Security.RolePermission;
using Contract.Security.UserPermission;

namespace Logic.Security
{
    public class SecurityLogicManager : ISecurityLogicManager
    {
        public IApplicationLogic Application { get; }
        // public IApplicationUserLogic ApplicationUser { get; }
        // public IPermissionLogic Permission { get; }
        // public IRoleLogic Role { get; }
        // public IRolePermissionLogic RolePermission { get; }
        // public IUserPermissionLogic UserPermission { get; }

        public SecurityLogicManager(
            IApplicationLogic applicationLogic//,
            // IApplicationUserLogic applicationUserLogic,
            // IPermissionLogic permissionLogic,
            // IRoleLogic roleLogic,
            // IRolePermissionLogic rolePermissionLogic,
            // IUserPermissionLogic userPermissionLogic
            )
        {
            Application = applicationLogic;
            // ApplicationUser = applicationUserLogic;
            // Permission = permissionLogic;
            // Role = roleLogic;
            // RolePermission = rolePermissionLogic;
            // UserPermission = userPermissionLogic;
        }
    }
}
