using Data.Security.Models;
using Dto.Security.ApplicationUser;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class ApplicationUserLogChangePasswordConverters
    {
        public static ApplicationUserLogChangePasswordDto ToDto(this ApplicationUserLogChangePassword source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserLogChangePasswordDto
            {
                ApplicationUserLogChangePasswordId = source.ApplicationUserLogChangePasswordId,
                ApplicationUserId = source.ApplicationUserId,
                ApplicationId = source.ApplicationId,
                OldPassword = source.OldPassword,
                CreatedOn = source.CreatedOn,
                CreatedBy = source.CreatedBy
            };

            return target;
        }
    
        public static async Task<List<ApplicationUserLogChangePasswordDto>> ToDtos(this IQueryable<ApplicationUserLogChangePassword> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

            return target;
        }
    }
}
