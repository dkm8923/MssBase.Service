using Data.Security.Models;
using Dto.Security.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class UserLogLoginConverters
    {
        public static UserLogLoginDto ToDto(this UserLogLogin source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new UserLogLoginDto
            {
                LogId = source.LogId,
                UserId = source.UserId,
                ApplicationId = source.ApplicationId,
                AuthToken = source.AuthToken,
                RefreshToken = source.RefreshToken,
                CreatedOn = source.CreatedOn,
                CreatedBy = source.CreatedBy
            };

            return target;
        }
    
        public static async Task<List<UserLogLoginDto>> ToDtos(this IQueryable<UserLogLogin> source, CancellationToken cancellationToken = default)
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
