using Data.Security.Models;
using Dto.Security.Role;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class RoleConverters
    {
        public static RoleDto ToDto(this Models.Role source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new RoleDto
            {
                RoleId = source.RoleId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                Name = source.Name,
                Description = source.Description,
                ApplicationId = source.ApplicationId
            };

            return target;
        }

        public static List<RoleDto> ToDtos(this IEnumerable<Models.Role> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.Role ToEntityOnInsert(this InsertUpdateRoleRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.Role
            {
                Active = source.Active,
                Name = source.Name,
                Description = source.Description,
                ApplicationId = source.ApplicationId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Models.Role UpdateEntityFromRequest(this Models.Role entity, InsertUpdateRoleRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.Name = source.Name;
            entity.Description = source.Description;
            entity.ApplicationId = source.ApplicationId;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
