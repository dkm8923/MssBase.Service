using MudBlazor.Services;
using Contract.Security.Permission;
using MssBase.UI.Configuration;
using MssBase.UI.HttpClients.Shared;    
using MssBase.UI.Components;
using MssBase.UI.HttpClients.Security;
using Contract.Security.Role;
using MssBase.UI.Components.Services;
using MudBlazor;
using Contract.Security.Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using MssBase.UI.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.Configure<SecurityApiOptions>(
    builder.Configuration.GetSection(SecurityApiOptions.SectionName));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ServerSessionStore>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "__Host-MssBase.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
    });
builder.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ServerSessionStore>((options, sessionStore) => options.SessionStore = sessionStore);
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CurrentSessionTokenProvider>();

builder.Services.AddHttpClient<IApplicationService, ApplicationHttpClient>();
builder.Services.AddHttpClient<IPermissionService, PermissionHttpClient>();

builder.Services.AddHttpClient<IRoleService, RoleHttpClient>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<PermissionStateService>();
builder.Services.AddScoped<LoadingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;
