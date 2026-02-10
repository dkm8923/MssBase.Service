using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MssBase.Service.Shared.JsonConverters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.Null)
                    throw new JsonException("A non-nullable DateOnly value was null.");

                var value = reader.GetString();

                if (string.IsNullOrWhiteSpace(value))
                    throw new JsonException("A non-nullable DateOnly value was empty or whitespace.");

                // Fast path for pure date-only strings (ISO and common formats)
                if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnlyResult))
                {
                    return dateOnlyResult;
                }

                // Attempt parse as DateTimeOffset (handles "Fri Dec 05 2025 06:42:38 GMT-0500 (Eastern Standard Time)" and other timezone formats)
                if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out var dto))
                {
                    return DateOnly.FromDateTime(dto.DateTime);
                }

                // Fallback to DateTime parse using invariant and current culture
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out var dt))
                {
                    return DateOnly.FromDateTime(dt);
                }

                if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out dt))
                {
                    return DateOnly.FromDateTime(dt);
                }

                // If value is a numeric timestamp (seconds or milliseconds since epoch), attempt conversion.
                if (long.TryParse(value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
                {
                    try
                    {
                        DateTimeOffset fromEpoch;
                        if (longVal > 10000000000L)
                            fromEpoch = DateTimeOffset.FromUnixTimeMilliseconds(longVal);
                        else
                            fromEpoch = DateTimeOffset.FromUnixTimeSeconds(longVal);

                        return DateOnly.FromDateTime(fromEpoch.DateTime);
                    }
                    catch
                    {
                        // ignore and fall through to error
                    }
                }

                throw new JsonException($"Unable to convert \"{value}\" to DateOnly.");
            }
            catch (JsonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new JsonException($"Unable to convert to DateOnly. Error: {ex.Message}", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
    }
}
//namespace MssBase.Service.Shared.JsonConverters
//{
//    public class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
//    {
//        public override DateOnly Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//        {
//            return DateOnly.Parse(reader.GetString()!);
//        }

//        public override void Write(System.Text.Json.Utf8JsonWriter writer, DateOnly value, System.Text.Json.JsonSerializerOptions options)
//        {
//            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
//        }
//    }
//}
