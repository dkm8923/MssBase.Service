using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MssBase.Service.Shared.JsonConverters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private static readonly string[] AcceptedFormats =
        {
            "MM/dd/yyyy",
            "M/d/yyyy"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("A non-nullable DateTime value was null.");

            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("A non-nullable DateTime value was empty or whitespace.");

            if (DateTime.TryParseExact(value, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate))
            {
                return exactDate;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind, out var parsedDate))
            {
                return parsedDate;
            }

            throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}