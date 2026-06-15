using Data.Common.Models;
using Dto.Common.CommonRelationalData;
using Shared.Logic.Common;
using System.Reflection;
using System.Text.Json;

namespace Data.Common.Converters
{
    public static class CommonRelationalDataConverters
    {
        private static readonly Dictionary<string, PropertyInfo> _referenceTypePropertyMap =
            typeof(FilterCommonRelationalDataDto)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(List<CommonRelationalDataDto>))
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        public static async Task<FilterCommonRelationalDataDto> ToDto(this IQueryable<CommonRelationalData> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var ret = new FilterCommonRelationalDataDto();
            
            foreach (var item in source)
            {
                var relationalRecords = JsonSerializer.Deserialize<List<CommonRelationalDataDto>>(item.Json);
                if (relationalRecords.NotNullAndHasRecords()
                    && _referenceTypePropertyMap.TryGetValue(item.ReferenceType, out var property))
                {
                    property.SetValue(ret, relationalRecords);
                }
            }

            return ret;
        }
    }
}
