using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using FluentValidation.Results;

namespace MssBase.Service.Shared.FluentValidation;

public class FluentValidationCustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    // Return errors in the same dictionary format as FluentValidation produces: { "Field": [ "msg" ] }
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        var original = validationProblemDetails?.Errors ?? new Dictionary<string, string[]>();

        // Aggregate messages for sanitized keys
        var aggregated = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in original)
        {
            var key = kv.Key ?? string.Empty;
            var sanitizedKey = SanitizeKey(key);

            if (string.IsNullOrWhiteSpace(sanitizedKey))
            {
                sanitizedKey = key;
            }

            if (!aggregated.TryGetValue(sanitizedKey, out var list))
            {
                list = new List<string>();
                aggregated[sanitizedKey] = list;
            }

            list.AddRange(kv.Value);
        }

        // Convert aggregated lists to arrays and remove duplicates
        var resultDict = aggregated.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Distinct().ToArray());

        return new BadRequestObjectResult(new { response = "Invalid Request", errors = resultDict });
    }

    private static string SanitizeKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;

        var sanitizedKey = key;

        // Remove leading JSONPath like $.Property or $['Property'] or $("Property")
        if (sanitizedKey.StartsWith("$."))
        {
            sanitizedKey = sanitizedKey.Substring(2);
        }
        else if (sanitizedKey.StartsWith("$["))
        {
            sanitizedKey = sanitizedKey.Substring(2);
            if (sanitizedKey.EndsWith("]"))
            {
                sanitizedKey = sanitizedKey[..^1];
            }

            sanitizedKey = sanitizedKey.Trim('\'', '"');
        }

        // Trim any leading dot
        sanitizedKey = sanitizedKey.TrimStart('.');

        return sanitizedKey;
    }

    public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults)
    {
        return Task.FromResult<IActionResult?>(CreateActionResult(context, validationProblemDetails));
    }

}
