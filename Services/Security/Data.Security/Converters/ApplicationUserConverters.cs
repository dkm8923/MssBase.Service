using Data.Security.Models;
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
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                Password = source.Password,
                PasswordResetRequired = source.PasswordResetRequired,
                LastLoginDate = source.LastLoginDate,
                LastPasswordChangeDate = source.LastPasswordChangeDate,
                LastLockoutDate = source.LastLockoutDate,
                FailedPasswordAttemptCount = source.FailedPasswordAttemptCount,
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

        public static ApplicationUserDto ToDtoWithoutPassword(this ApplicationUser source)
        {
            if (source == null)
            {
                return null;
            }

            source.Password = null;
            return source.ToDto();
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

        public static async Task<List<ApplicationUserDto>> ToDtosWithoutPassword(this IQueryable<ApplicationUser> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDtoWithoutPassword()).ToListAsync(cancellationToken);

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
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                ApplicationId = source.ApplicationId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static ApplicationUser UpdateEntityFromRequest(this ApplicationUser entity, InsertUpdateApplicationUserRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.Email = source.Email;
            entity.FirstName = source.FirstName;
            entity.LastName = source.LastName;
            entity.DateOfBirth = source.DateOfBirth;
            entity.ApplicationId = source.ApplicationId;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
