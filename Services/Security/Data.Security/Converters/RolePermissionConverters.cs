using Data.Security.Models;
using Dto.Security.RolePermission;
using Shared.Logic.Common;

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
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                ApplicationId = source.ApplicationId,
                RoleId = source.RoleId,
                PermissionId = source.PermissionId
            };

            return target;
        }

        public static List<RolePermissionDto> ToDtos(this IEnumerable<Models.RolePermission> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

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
                PermissionId = source.PermissionId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

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
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
