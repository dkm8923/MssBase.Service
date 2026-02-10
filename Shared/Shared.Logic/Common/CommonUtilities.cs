using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace Shared.Logic.Common
{
    public static class CommonUtilities
    {
        /// <summary>
        /// Gets the current date and time in Coordinated Universal Time (UTC).
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value representing the current UTC date and time.</returns>
        public static DateTime GetDateTimeUtcNow()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether the specified input is a generic collection type (List<> or IEnumerable<>) and contains
        /// no elements.
        /// </summary>
        /// <remarks>This method checks only for generic List<> and IEnumerable<> types. If the input is
        /// not a supported collection type or is not empty, the method returns false.</remarks>
        /// <typeparam name="T">The type of the input to check. Typically expected to be a collection type.</typeparam>
        /// <param name="input">The object to evaluate for being an empty generic collection.</param>
        /// <returns>true if the input is a generic List<> or IEnumerable<> and contains no elements; otherwise, false.</returns>
        public static bool IsGenericTypeCollectionWithData<T>(T input)
        {
            var type = typeof(T);

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                // Cast to IList to check Count
                if (input is System.Collections.IList list)
                {
                    if (list.Count == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all whitespace characters from a string, including spaces, tabs, and newlines.
        /// </summary>
        /// <param name="input">The string to remove whitespace from. Can be null.</param>
        /// <returns>A new string with all whitespace removed, or null if the input is null.</returns>
        public static string RemoveWhiteSpaceFromString(string input)
        {
            if (input == null)
            {
                return null;
            }

            return Regex.Replace(input, @"\s+", "");
        }

        /// <summary>
        /// Retrieves the local IPv4 address of the current machine.
        /// </summary>
        /// <returns>The first IPv4 address found, or "255.255.255.255" (IPAddress.None) if no IPv4 address is available.</returns>
        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork))
            {
                return ip.ToString();
            }
            return IPAddress.None.ToString();
        }

        /// <summary>
        /// Validates whether a string is a properly formatted Base64-encoded string.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <returns>True if the string is valid Base64; otherwise, false.</returns>
        public static bool IsBase64String(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Normalize padding
            value = value.Trim();
            if (value.Length % 4 != 0)
                return false;

            try
            {
                // Try to convert from Base64
                Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}
