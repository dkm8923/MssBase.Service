using Data.Security.Models;
using Dto.Security.Application;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class ApplicationConverters
    {
        public static ApplicationDto ToDto(this Models.Application source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationDto
            {
                ApplicationId = source.ApplicationId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                Name = source.Name,
                Description = source.Description
            };

            if (source.ApplicationUsers != null)
            {
                target.ApplicationUsers = source.ApplicationUsers.Select(au => au.ToDto());
            }

            if (source.Permissions != null)
            {
                target.Permissions = source.Permissions.Select(p => p.ToDto());
            }

            if (source.Roles != null)
            {
                target.Roles = source.Roles.Select(r => r.ToDto());
            }

            if (source.RolePermissions != null)
            {
                target.RolePermissions = source.RolePermissions.Select(rp => rp.ToDto());
            }

            if (source.ApplicationUserPermissions != null)
            {
                target.ApplicationUserPermissions = source.ApplicationUserPermissions.Select(aup => aup.ToDto());
            }

            return target;
        }

        public static List<ApplicationDto> ToDtos(this IEnumerable<Models.Application> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.Application ToEntityOnInsert(this InsertUpdateApplicationRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.Application
            {
                Active = source.Active,
                Name = source.Name,
                Description = source.Description
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Models.Application UpdateEntityFromRequest(this Models.Application entity, InsertUpdateApplicationRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.Name = source.Name;
            entity.Description = source.Description;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
