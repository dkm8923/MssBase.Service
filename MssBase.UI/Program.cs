using MudBlazor.Services;
using Contract.Security.Permission;
using MssBase.UI.Configuration;
using MssBase.UI.HttpClients.Shared;    
using MssBase.UI.Components;
using MssBase.UI.HttpClients.Security;
using Contract.Security.Role;
using MssBase.UI.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.Configure<SecurityApiOptions>(
    builder.Configuration.GetSection(SecurityApiOptions.SectionName));

builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddHttpClient<IPermissionService, PermissionHttpClient>()
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IRoleService, RoleHttpClient>()
    .AddHttpMessageHandler<BearerTokenHandler>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// builder.Services.AddSingleton<ToastService>();
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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
