using MssBase.Service;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

// Configure OpenApi
builder.Services.AddOpenApi();

builder.ConfigureLogging();

// Add services to the container.
builder.Services.ConfigureCache(builder);

builder.Services.AddHttpClient();

builder.Services.ConfigureAuthenticationSettings(builder);

builder.Services.ConfigureJwtAuthentication(builder);
builder.Services.AddPermissionAuthorization();

builder.Services.ConfigureControllers(builder);

builder.Services.ConfigureCors(builder);

builder.Services.ConfigureLoggerService(builder, environment);

//builder.Services.ConfigureCommonService(builder);
builder.Services.ConfigureSecurityService(builder);

builder.Services.ConfigureFluentValidationAutoValidation(builder);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
