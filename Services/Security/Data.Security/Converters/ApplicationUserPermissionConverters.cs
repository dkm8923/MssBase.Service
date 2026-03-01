using Data.Security.Models;
using Dto.Security.ApplicationUserPermission;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class ApplicationUserPermissionConverters
    {
        public static ApplicationUserPermissionDto ToDto(this Models.ApplicationUserPermission source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserPermissionDto
            {
                ApplicationUserPermissionId = source.ApplicationUserPermissionId,
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

        public static List<ApplicationUserPermissionDto> ToDtos(this IEnumerable<Models.ApplicationUserPermission> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.ApplicationUserPermission ToEntityOnInsert(this InsertUpdateApplicationUserPermissionRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.ApplicationUserPermission
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

        public static Models.ApplicationUserPermission UpdateEntityFromRequest(this Models.ApplicationUserPermission entity, InsertUpdateApplicationUserPermissionRequest source)
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
