using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MssBase.Service.Shared.JsonConverters
{
    public class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                var value = reader.GetString();

                if (string.IsNullOrWhiteSpace(value))
                    return null;

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
                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
                {
                    // heuristics: > 10_000_000_000 likely milliseconds; else seconds
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

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            else
                writer.WriteNullValue();
        }
    }
}

//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace MssBase.Service.Shared.JsonConverters
//{
//    public class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
//    {
//        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            //convert different date formats to DateOnly
//            try
//            {
//                if (reader.TokenType == JsonTokenType.Null)
//                    return null;

//                var value = reader.GetString();

//                if (string.IsNullOrWhiteSpace(value))
//                    return null;

//                var result = DateOnly.Parse(value);
//                return result;
//            }
//            catch (Exception ex)
//            {
//                throw new JsonException($"Unable to convert to DateOnly. Error: {ex.Message}", ex);
//            }
//        }

//        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
//        {
//            if (value.HasValue)
//                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
//            else
//                writer.WriteNullValue();
//        }
//    }
//}
