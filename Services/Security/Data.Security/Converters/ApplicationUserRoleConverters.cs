using Data.Security.Models;
using Dto.Security.ApplicationUserRole;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class ApplicationUserRoleConverters
    {
        public static ApplicationUserRoleDto ToDto(this Models.ApplicationUserRole source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserRoleDto
            {
                ApplicationUserRoleId = source.ApplicationUserRoleId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                ApplicationId = source.ApplicationId,
                ApplicationUserId = source.ApplicationUserId,
                RoleId = source.RoleId
            };

            return target;
        }

        public static List<ApplicationUserRoleDto> ToDtos(this IEnumerable<Models.ApplicationUserRole> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.ApplicationUserRole ToEntityOnInsert(this InsertUpdateApplicationUserRoleRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.ApplicationUserRole
            {
                Active = source.Active,
                ApplicationId = source.ApplicationId,
                ApplicationUserId = source.ApplicationUserId,
                RoleId = source.RoleId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Models.ApplicationUserRole UpdateEntityFromRequest(this Models.ApplicationUserRole entity, InsertUpdateApplicationUserRoleRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.ApplicationId = source.ApplicationId;
            entity.ApplicationUserId = source.ApplicationUserId;
            entity.RoleId = source.RoleId;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
