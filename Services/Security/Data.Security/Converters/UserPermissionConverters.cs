using Data.Security.Models;
using Dto.Security.UserPermission;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class UserPermissionConverters
    {
        public static UserPermissionDto ToDto(this Models.UserPermission source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new UserPermissionDto
            {
                UserPermissionId = source.UserPermissionId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                ApplicationId = source.ApplicationId,
                ApplicationUserId = source.ApplicationUserId,
                PermissionId = source.PermissionId
            };

            return target;
        }

        public static List<UserPermissionDto> ToDtos(this IEnumerable<Models.UserPermission> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.UserPermission ToEntityOnInsert(this InsertUpdateUserPermissionRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.UserPermission
            {
                Active = source.Active,
                ApplicationId = source.ApplicationId,
                ApplicationUserId = source.ApplicationUserId,
                PermissionId = source.PermissionId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Models.UserPermission UpdateEntityFromRequest(this Models.UserPermission entity, InsertUpdateUserPermissionRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.ApplicationId = source.ApplicationId;
            entity.ApplicationUserId = source.ApplicationUserId;
            entity.PermissionId = source.PermissionId;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
