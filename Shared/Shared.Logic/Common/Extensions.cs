namespace Shared.Logic.Common
{
    //Place all shared extension methods here...
    public static class Extensions
    {
        /// <summary>
        /// Extension method for IEnumerable<T> to check if it is not null and has records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool NotNullAndHasRecords<T>(this IEnumerable<T>? source)
        {
            if (source is null) return false;

            // Fast path when Count is available
            if (source is ICollection<T> collection) return collection.Count > 0;
            if (source is IReadOnlyCollection<T> readOnlyCollection) return readOnlyCollection.Count > 0;

            // Fallback for true enumerables
            return source.Any();
        }

        /// <summary>
        /// Extension method for List<T> to check if it is not null and has records. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool NotNullAndHasRecords<T>(this List<T>? source)
        {
            return source is { Count: > 0 };
        }
    }
}
