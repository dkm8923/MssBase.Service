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
    }
}
