using Data.Security.Models;
using Dto.Security.Permission;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class PermissionConverters
    {
        public static PermissionDto ToDto(this Models.Permission source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new PermissionDto
            {
                PermissionId = source.PermissionId,
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

        public static List<PermissionDto> ToDtos(this IEnumerable<Models.Permission> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.Permission ToEntityOnInsert(this InsertUpdatePermissionRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.Permission
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

        public static Models.Permission UpdateEntityFromRequest(this Models.Permission entity, InsertUpdatePermissionRequest source)
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
