using System;
using Dto.Security.Application;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserRole;
using Dto.Security.Permission;
using Dto.Security.Role;
using Dto.Security.RolePermission;

namespace IntegrationTests.Security.Shared;

public class SecurityTestData
{
    public SecurityTestData()
    {
        ActiveApplications = new List<ApplicationDto>();
        InactiveApplications = new List<ApplicationDto>();
        ActiveApplicationUsers = new List<ApplicationUserDto>();
        InactiveApplicationUsers = new List<ApplicationUserDto>();
        ActiveApplicationUserRoles = new List<ApplicationUserRoleDto>();
        InactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();
        ActivePermissions = new List<PermissionDto>();
        InactivePermissions = new List<PermissionDto>();
        ActiveRoles = new List<RoleDto>();
        InactiveRoles = new List<RoleDto>();
        ActiveRolePermissions = new List<RolePermissionDto>();
        InactiveRolePermissions = new List<RolePermissionDto>();
        ActiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        InactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
    }

    public List<ApplicationDto> ActiveApplications { get; set; }
    public List<ApplicationDto> InactiveApplications { get; set; }
    public List<ApplicationUserDto> ActiveApplicationUsers { get; set; }
    public List<ApplicationUserDto> InactiveApplicationUsers { get; set; }
    public List<ApplicationUserRoleDto> ActiveApplicationUserRoles { get; set; }
    public List<ApplicationUserRoleDto> InactiveApplicationUserRoles { get; set; }
    public List<PermissionDto> ActivePermissions { get; set; }
    public List<PermissionDto> InactivePermissions { get; set; }
    public List<RoleDto> ActiveRoles { get; set; }
    public List<RoleDto> InactiveRoles { get; set; }
    public List<RolePermissionDto> ActiveRolePermissions { get; set; }
    public List<RolePermissionDto> InactiveRolePermissions { get; set; }
    public List<ApplicationUserPermissionDto> ActiveApplicationUserPermissions { get; set; }
    public List<ApplicationUserPermissionDto> InactiveApplicationUserPermissions { get; set; }
}