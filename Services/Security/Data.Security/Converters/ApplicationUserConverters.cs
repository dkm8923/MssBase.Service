using Data.Security.Models;
using Dto.Security.Application;
using Dto.Security.ApplicationUser;
using Shared.Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class ApplicationUserConverters
    {
        public static ApplicationUserDto ToDto(this ApplicationUser source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserDto
            {
                ApplicationUserId = source.ApplicationUserId,
                Active = source.Active,
                ReadOnly = source.ReadOnly,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                UserId = source.UserId,
                ApplicationId = source.ApplicationId
            };

            if (source.ApplicationUserPermissions.NotNullAndHasRecords())
            {
                target.ApplicationUserPermissions = source.ApplicationUserPermissions.Select(au => au.ToDto());
            }

            if (source.ApplicationUserRoles.NotNullAndHasRecords())
            {
                target.ApplicationUserRoles = source.ApplicationUserRoles.Select(au => au.ToDto());
            }

            return target;
        }

        public static async Task<List<ApplicationUserDto>> ToDtos(this IQueryable<ApplicationUser> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

            return target;
        }

        public static ApplicationUser ToEntityOnInsert(this InsertUpdateApplicationUserRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUser
            {
                Active = source.Active,
                UserId = source.UserId,
                ApplicationId = source.ApplicationId,
                CurrentUser = source.CurrentUser
            };

            return target;
        }

        public static ApplicationUser UpdateEntityFromRequest(this ApplicationUser entity, InsertUpdateApplicationUserRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.UserId = source.UserId;
            entity.ApplicationId = source.ApplicationId;
            entity.CurrentUser = source.CurrentUser;

            return entity;
        }
    }
}
