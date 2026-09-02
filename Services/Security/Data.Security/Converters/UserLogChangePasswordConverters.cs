using Data.Security.Models;
using Dto.Security.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Converters
{
    public static class UserLogChangePasswordConverters
    {
        public static UserLogChangePasswordDto ToDto(this UserLogChangePassword source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new UserLogChangePasswordDto
            {
                LogId = source.LogId,
                UserId = source.UserId,
                OldPassword = source.OldPassword,
                CreatedOn = source.CreatedOn,
                CreatedBy = source.CreatedBy
            };

            return target;
        }
    
        public static async Task<List<UserLogChangePasswordDto>> ToDtos(this IQueryable<UserLogChangePassword> source, CancellationToken cancellationToken = default)
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
