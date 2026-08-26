using Data.Security.Models;
using Dto.Security.RolePermission;
using Shared.Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class RolePermissionConverters
    {
        public static RolePermissionDto ToDto(this Models.RolePermission source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new RolePermissionDto
            {
                RolePermissionId = source.RolePermissionId,
                Active = source.Active,
                ReadOnly = source.ReadOnly,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                ApplicationId = source.ApplicationId,
                RoleId = source.RoleId,
                PermissionId = source.PermissionId
            };

            if (source.Permission != null)
            {
                target.Permission = source.Permission.ToDto();
            }

            return target;
        }

        public static async Task<List<RolePermissionDto>> ToDtos(this IQueryable<Models.RolePermission> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

            return target;
        }

        public static Models.RolePermission ToEntityOnInsert(this InsertUpdateRolePermissionRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.RolePermission
            {
                Active = source.Active,
                ApplicationId = source.ApplicationId,
                RoleId = source.RoleId,
                PermissionId = source.PermissionId,
                CurrentUser = source.CurrentUser
            };

            return target;
        }

        public static Models.RolePermission UpdateEntityFromRequest(this Models.RolePermission entity, InsertUpdateRolePermissionRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.ApplicationId = source.ApplicationId;
            entity.RoleId = source.RoleId;
            entity.PermissionId = source.PermissionId;
            entity.CurrentUser = source.CurrentUser;

            return entity;
        }
    }
}
