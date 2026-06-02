using FluentAssertions;
using System.Text;

namespace IntegrationTests.Shared
{
    public static class LogicTestUtilities
    {
        /// <summary>
        /// Generates a random alphanumeric string of the specified length.
        /// </summary>
        /// <param name="length">The number of characters in the generated string. Must be non-negative.</param>
        /// <returns>A randomly generated string consisting of uppercase letters, lowercase letters, and digits. The length of
        /// the string is equal to the specified value of <paramref name="length"/>.</returns>
        public static string GenerateRandomString(int length)
        {
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a random 16-bit signed integer within the specified range.
        /// </summary>
        /// <param name="rangeStart">The inclusive lower bound of the random number to generate. If null, <see cref="Int16.MinValue"/> is used.</param>
        /// <param name="rangeEnd">The inclusive upper bound of the random number to generate. If null, <see cref="Int16.MaxValue"/> is used.</param>
        /// <returns>A random 16-bit signed integer greater than or equal to <paramref name="rangeStart"/> and less than or equal
        /// to <paramref name="rangeEnd"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rangeStart"/> is greater than <paramref name="rangeEnd"/>.</exception>
        public static short GenerateRandomShort(short? rangeStart = null, short? rangeEnd = null)
        {
            var random = new Random();

            short min = rangeStart ?? short.MinValue;
            short max = rangeEnd ?? short.MaxValue;

            if (min > max)
            {
                throw new ArgumentException("rangeStart cannot be greater than rangeEnd");
            }

            return (short)random.Next(min, max + 1);
        }

        /// <summary>
        /// Returns a randomly generated boolean value with equal probability of true or false.
        /// </summary>
        /// <returns>A random boolean value (true or false).</returns>
        /// <remarks>
        /// This method uses <see cref="Random.Shared"/> which is thread-safe and suitable for 
        /// most scenarios. Each call has a 50% chance of returning true and 50% chance of returning false.
        /// </remarks>
        public static bool GenerateRandomBool()
        {
            return Random.Shared.Next(2) == 0;
        }

        /// <summary>
        /// Validates that the actual error results match the expected field errors for each field.
        /// </summary>
        /// <param name="expectedFieldErrors">A dictionary containing the expected error messages for each field, where the key is the field name and the
        /// value is a list of expected error messages.</param>
        /// <param name="actualErrors">A dictionary containing the actual error messages for each field, where the key is the field name and the
        /// value is a list of actual error messages to be validated.</param>
        public static void VerifyLogicErrorResultsAreValid(Dictionary<string, List<string>> expectedFieldErrors, Dictionary<string, List<string>> actualErrors)
        {
            foreach (var expected in expectedFieldErrors)
            {
                // Verify the error dictionary contains the expected key
                actualErrors.Should().ContainKey(expected.Key);

                // Verify the list of error messages matches the expected list for that key
                actualErrors[expected.Key].Should().BeEquivalentTo(expected.Value);
            }
        }

        /// <summary>
        /// Gets today's date in UTC as a DateOnly object.
        /// </summary>
        /// <returns></returns>
        public static DateOnly GetTodaysUtcDateOnly()
        {
            return new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
        }

        /// <summary>
        /// Returns a randomly generated date within the specified year.
        /// </summary>
        /// <param name="year">The year for which to generate a random date.</param>
        /// <returns>A randomly generated <see cref="DateOnly"/> within the specified year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateOnly GetRandomDateForYear(int year)
        {
            // Ensure the year is valid to prevent exceptions
            if (year < DateOnly.MinValue.Year || year > DateOnly.MaxValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year is out of the supported range.");
            }

            // Determine the start of the year (January 1)
            var startOfYear = new DateOnly(year, 1, 1);

            // Determine the end of the year, accounting for leap years
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            // Generate a random number of days to add (0 to daysInYear - 1)
            int randomDays = Random.Shared.Next(daysInYear);

            return startOfYear.AddDays(randomDays);
        }

        /// <summary>
        /// Generates a random DateTime value for a specific year including random time components.
        /// </summary>
        /// <param name="year">The target year (e.g., 2026).</param>
        /// <returns>A random DateTime within the specified year.</returns>
        public static DateTime GetRandomDateTime(int year)
        {
            // 1. Define the exact start of the year
            DateTime startOfYear = new DateTime(year, 1, 1);
            
            // 2. Determine if it is a leap year to get the correct total days (365 or 366)
            int totalDaysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            
            // 3. Convert the maximum range into seconds to allow for random time components
            int totalSecondsInYear = totalDaysInYear * 24 * 60 * 60;
            
            // 4. Generate a random second offset (upper bound is exclusive)
            int randomSecondsOffset = Random.Shared.Next(0, totalSecondsInYear);
            
            // 5. Return the resulting random DateTime
            return startOfYear.AddSeconds(randomSecondsOffset);
        }
    }
}
