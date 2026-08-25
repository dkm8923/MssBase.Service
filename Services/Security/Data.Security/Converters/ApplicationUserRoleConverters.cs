using Data.Security.Models;
using Dto.Security.ApplicationUserRole;
using Shared.Logic.Common;
using Microsoft.EntityFrameworkCore;

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
                ReadOnly = source.ReadOnly,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                ApplicationId = source.ApplicationId,
                ApplicationUserId = source.ApplicationUserId,
                RoleId = source.RoleId
            };

            if (source.Role != null)
            {
                target.Role = source.Role.ToDto();
            }

            return target;
        }

        public static async Task<List<ApplicationUserRoleDto>> ToDtos(this IQueryable<Models.ApplicationUserRole> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

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
                RoleId = source.RoleId,
                CurrentUser = source.CurrentUser
            };

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
            entity.CurrentUser = source.CurrentUser;

            return entity;
        }
    }
}
