using MssBase.Service;
using MssBase.Service.Shared.JsonConverters;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

// Configure OpenApi
builder.Services.AddOpenApi();

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.ConfigureCache(builder);

builder.Services.AddHttpClient();

builder.Services.AddControllers(config =>
{
    config.Filters.Add(new ProducesAttribute("application/json"));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.ConfigureBaseDependencies(builder, environment);

//builder.Services.ConfigureCommonService(builder);

builder.Services.ConfigureSecurityService(builder);

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    // Disable the built-in .NET model (data annotations) validation.
    configuration.DisableBuiltInModelValidation = true;

    // Only validate controllers decorated with the `AutoValidation` attribute.
    configuration.ValidationStrategy = ValidationStrategy.Annotations;

    // Enable validation for parameters bound from `BindingSource.Body` binding sources.
    configuration.EnableBodyBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Form` binding sources.
    configuration.EnableFormBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Query` binding sources.
    configuration.EnableQueryBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from `BindingSource.Path` binding sources.
    configuration.EnablePathBindingSourceAutomaticValidation = true;

    // Enable validation for parameters bound from 'BindingSource.Custom' binding sources.
    configuration.EnableCustomBindingSourceAutomaticValidation = true;

    // Replace the default result factory with a custom implementation.
    configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
});

// Add MicroElements FluentValidation -> Swagger mapping
//builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRouting(); // - Required for CORS to work properly

app.UseCors("AppPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
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
}