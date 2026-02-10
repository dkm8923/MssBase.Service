using Shared.Logic.Common;

namespace Shared.Service.Cache
{
    public static class CacheUtilities
    {
        #region Private

        //cache key names
        private static string CacheKeyGetAll = "GetAll";
        private static string CacheKeyGetById = "GetById";
        private static string CacheKeyFilter = "Filter";

        #endregion

        public static bool KeySatisfiesPattern(string key, string pattern)
        {
            return key.StartsWith(pattern);
        }

        /// <summary>
        /// Creates cache key for "GetAll" requests
        /// </summary>
        /// <param name="cacheKeySectionName">Service Name</param>
        /// <returns></returns>
        public static string CreateGetAllCacheKey(string cacheKeySectionName, bool includeInactive = false, bool includeRelated = false)
        {
            var includeInactiveKey = (includeInactive ? 1 : 0).ToString();
            var includeRelatedKey = (includeRelated ? 1 : 0).ToString();
            return $"{cacheKeySectionName}_{CacheKeyGetAll}_{includeInactiveKey}_{includeRelatedKey}";
        }

        /// <summary>
        /// Creates cache key for "GetById" requests
        /// </summary>
        /// <param name="cacheKeySectionName">Service Name</param>
        /// <param name="id">Id used to retrieve single record</param>
        /// <returns></returns>
        public static string CreateGetByIdCacheKey(string cacheKeySectionName, long id, bool includeInactive = false, bool includeRelated = false)
        {
            var includeInactiveKey = (includeInactive ? 1 : 0).ToString();
            var includeRelatedKey = (includeRelated ? 1 : 0).ToString();
            return $"{cacheKeySectionName}_{CacheKeyGetById}_{id}_{includeInactiveKey}_{includeRelatedKey}";
        }

        /// <summary>
        /// Creates cache key for "Filter" requests
        /// </summary>
        /// <param name="cacheKeySectionName">Service Name</param>
        /// <param name="cacheParms">Filter Parm key values</param>
        /// <returns></returns>
        public static string CreateFilterCacheKey(string cacheKeySectionName, List<string> cacheParms)
        {
            var cacheKeyName = $"{cacheKeySectionName}_{CacheKeyFilter}";

            foreach (var parm in cacheParms)
            {
                cacheKeyName += "_" + parm;
            }

            return cacheKeyName;
        }

        /// <summary>
        /// Creates a key by removing all whitespace characters from the specified string.
        /// </summary>
        /// <param name="val">The input string from which to generate the key. Can be null.</param>
        /// <returns>A string with all whitespace removed from the input. Returns "0" if <paramref name="val"/> is null.</returns>
        public static string CreateKeyFromString(string val)
        {
            if (val != null)
            {
                return CommonUtilities.RemoveWhiteSpaceFromString(val);
            }

            return "0";
        }

        /// <summary>
        /// Creates a string key representation from the specified nullable integer value.
        /// </summary>
        /// <param name="val">The nullable integer value to convert to a string key. If null, the key will be "0".</param>
        /// <returns>A string containing the value of <paramref name="val"/> if it has a value; otherwise, "0".</returns>
        public static string CreateKeyFromInt(int? val)
        {
            if (val.HasValue)
            {
                return val.ToString();
            }

            return "0";
        }

        /// <summary>
        /// Creates a string key representation from the specified nullable integer value.
        /// </summary>
        /// <param name="val">The nullable integer value to convert to a string key. If null, the key will be "0".</param>
        /// <returns>A string containing the value of <paramref name="val"/> if it has a value; otherwise, "0".</returns>
        public static string CreateKeyFromLong(long? val)
        {
            if (val.HasValue)
            {
                return val.ToString();
            }

            return "0";
        }

        /// <summary>
        /// Creates a string key representation from a nullable decimal value.
        /// </summary>
        /// <param name="val">The nullable decimal value to convert to a string key. If null, a default key is returned.</param>
        /// <returns>A string representation of the decimal value if <paramref name="val"/> has a value; otherwise, "0".</returns>
        public static string CreateKeyFromDecimal(decimal? val)
        {
            if (val.HasValue)
            {
                return val.ToString();
            }

            return "0";
        }

        /// <summary>
        /// Converts a nullable Boolean value to a string key representation.
        /// </summary>
        /// <param name="val">The nullable Boolean value to convert. If <see langword="true"/>, the method returns "1"; otherwise, it
        /// returns "0".</param>
        /// <returns>A string representing the Boolean value: "1" if <paramref name="val"/> is <see langword="true"/>; otherwise,
        /// "0".</returns>
        public static string CreateKeyFromBool(bool? val)
        {
            if (val != null && val == true)
            {
                return "1";
            }

            return "0";
        }

        /// <summary>
        /// Creates a string key representation from a specified nullable date value.
        /// </summary>
        /// <param name="val">The date value to convert to a string key. If null, a default key is returned.</param>
        /// <returns>A string in the format "yyyy-MM-dd" representing the date if <paramref name="val"/> has a value; otherwise,
        /// "0".</returns>
        public static string CreateKeyFromDateOnly(DateOnly? val)
        {
            if (val.HasValue)
            {
                return val.Value.ToString("yyyy-MM-dd");
            }

            return "0";
        }

        /// <summary>
        /// Creates a string key representation from the specified <see cref="DateTime"/> value using the format
        /// "yyyyMMddHHmmss".
        /// </summary>
        /// <param name="val">The <see cref="DateTime"/> value to convert to a key. If <see langword="null"/>, a default key is returned.</param>
        /// <returns>A string representing the date and time in "yyyyMMddHHmmss" format if <paramref name="val"/> has a value;
        /// otherwise, "0".</returns>
        public static string CreateKeyFromDateTime(DateTime? val)
        {
            if (val.HasValue)
            {
                return val.Value.ToString("yyyyMMddHHmmss");
            }

            return "0";
        }
    }
}
