using Data.Security.Models;
using Dto.Security.ApplicationUser;
using Shared.Logic.Common;
using Microsoft.EntityFrameworkCore;
using Dto.Security.User;

namespace Data.Security.Converters
{
    public static class UserConverters
    {
        public static UserDto ToDto(this User source)
        {
            if (source == null)
            {
                return null;
            }

            var applicationUserLogin = source.UserLogin != null ? source.UserLogin : new UserLogin();

            var target = new UserDto
            {
                UserId = source.UserId,
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
                Password = applicationUserLogin.Password,
                PasswordResetRequired = applicationUserLogin.PasswordResetRequired,
                LastLoginDate = applicationUserLogin.LastLoginDate,
                LastPasswordChangeDate = applicationUserLogin.LastPasswordChangeDate,
                LastLockoutDate = applicationUserLogin.LastLockoutDate,
                FailedPasswordAttemptCount = applicationUserLogin.FailedPasswordAttemptCount,
            };

            // if (source.ApplicationUserPermissions.NotNullAndHasRecords())
            // {
            //     target.ApplicationUserPermissions = source.ApplicationUserPermissions.Select(au => au.ToDto());
            // }

            // if (source.ApplicationUserRoles.NotNullAndHasRecords())
            // {
            //     target.ApplicationUserRoles = source.ApplicationUserRoles.Select(au => au.ToDto());
            // }

            return target;
        }

        public static UserDto ToDtoWithoutPassword(this User source)
        {
            if (source == null)
            {
                return null;
            }

            source.Password = null;
            return source.ToDto();
        }

        public static async Task<List<UserDto>> ToDtos(this IQueryable<User> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

            return target;
        }

        public static async Task<List<UserDto>> ToDtosWithoutPassword(this IQueryable<User> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDtoWithoutPassword()).ToListAsync(cancellationToken);

            return target;
        }

        public static User ToEntityOnInsert(this InsertUpdateUserRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new User
            {
                Active = source.Active,
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                CurrentUser = source.CurrentUser
            };

            return target;
        }

        public static User UpdateEntityFromRequest(this User entity, InsertUpdateUserRequest source)
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
            entity.CurrentUser = source.CurrentUser;

            return entity;
        }
    }
}
