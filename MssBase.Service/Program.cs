using MssBase.Service;
using Scalar.AspNetCore;
using Serilog;

// Bootstrap logger captures startup failures before appsettings.json is loaded.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting MssBase.Service");

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

    builder.Services.ConfigurePasswordValidationSettings(builder);

    builder.Services.AddPermissionAuthorization();

    builder.Services.ConfigureControllers(builder);

    builder.Services.ConfigureCors(builder);

    builder.Services.ConfigureLoggerService(builder, environment);

    builder.Services.ConfigureCommonService(builder);
    builder.Services.ConfigureSecurityService(builder);
    builder.Services.ConfigureSharedServiceDependencies(builder);

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

    //collapses the many framework log lines per HTTP request into one structured summary event (method, path, status code, elapsed ms), which is far cheaper to query in Seq.
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseRouting(); // - Required for CORS to work properly

    app.UseCors("AppPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MssBase.Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
