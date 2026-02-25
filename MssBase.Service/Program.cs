using MssBase.Service;
using MssBase.Service.Shared.JsonConverters;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.OpenApi.Models;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using MssBase.Service.Shared.FluentValidation;

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
    configuration.OverrideDefaultResultFactoryWith<FluentValidationCustomResultFactory>();
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
