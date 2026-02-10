using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Security.Permission;
using Contract.Security.Role;
using Contract.Security.RolePermission;
using Contract.Security.UserPermission;

namespace Contract.Security
{
    public interface ISecurityLogicManager
    {
        public IApplicationLogic Application { get; }
        // public IApplicationUserLogic ApplicationUser { get; }
        // public IPermissionLogic Permission { get; }
        // public IRoleLogic Role { get; }
        // public IRolePermissionLogic RolePermission { get; }
        // public IUserPermissionLogic UserPermission { get; }
    }
}
