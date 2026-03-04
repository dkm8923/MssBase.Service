using Data.Security.Models;
using Dto.Security.ApplicationUser;
using Shared.Logic.Common;

namespace Data.Security.Converters
{
    public static class ApplicationUserConverters
    {
        public static ApplicationUserDto ToDto(this Models.ApplicationUser source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserDto
            {
                ApplicationUserId = source.ApplicationUserId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                Password = source.Password,
                LastLoginDate = source.LastLoginDate,
                LastPasswordChangeDate = source.LastPasswordChangeDate,
                LastLockoutDate = source.LastLockoutDate,
                FailedPasswordAttemptCount = source.FailedPasswordAttemptCount,
                ApplicationId = source.ApplicationId
            };

            if (source.ApplicationUserPermissions != null)
            {
                target.ApplicationUserPermissions = source.ApplicationUserPermissions.Select(au => au.ToDto());
            }

            return target;
        }

        public static List<ApplicationUserDto> ToDtos(this IEnumerable<Models.ApplicationUser> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Models.ApplicationUser ToEntityOnInsert(this InsertUpdateApplicationUserRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Models.ApplicationUser
            {
                Active = source.Active,
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                Password = source.Password,
                LastLoginDate = source.LastLoginDate,
                LastPasswordChangeDate = source.LastPasswordChangeDate,
                LastLockoutDate = source.LastLockoutDate,
                FailedPasswordAttemptCount = source.FailedPasswordAttemptCount,
                ApplicationId = source.ApplicationId
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Models.ApplicationUser UpdateEntityFromRequest(this Models.ApplicationUser entity, InsertUpdateApplicationUserRequest source)
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
            entity.Password = source.Password;
            entity.LastLoginDate = source.LastLoginDate;
            entity.LastPasswordChangeDate = source.LastPasswordChangeDate;
            entity.LastLockoutDate = source.LastLockoutDate;
            entity.FailedPasswordAttemptCount = source.FailedPasswordAttemptCount;
            entity.ApplicationId = source.ApplicationId;
            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
