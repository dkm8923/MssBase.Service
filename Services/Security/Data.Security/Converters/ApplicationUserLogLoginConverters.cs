using Data.Security.Models;
using Dto.Security.ApplicationUser;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class ApplicationUserLogLoginConverters
    {
        public static ApplicationUserLogLoginDto ToDto(this ApplicationUserLogLogin source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ApplicationUserLogLoginDto
            {
                LogId = source.LogId,
                ApplicationUserId = source.ApplicationUserId,
                ApplicationId = source.ApplicationId,
                AuthToken = source.AuthToken,
                RefreshToken = source.RefreshToken,
                CreatedOn = source.CreatedOn,
                CreatedBy = source.CreatedBy
            };

            return target;
        }
    
        public static async Task<List<ApplicationUserLogLoginDto>> ToDtos(this IQueryable<ApplicationUserLogLogin> source, CancellationToken cancellationToken = default)
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
