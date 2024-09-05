using System.Text;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Serilog;
using WebApp;
using WebApp.Authentication;
using WebApp.Components;

Logging.InitializeLogging();

try
{
    Log.Information("Creating Application builder");
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMudServices();
    
    builder.Services.ConfigureLogging();

    builder.Services.AddScoped(
        scope =>
        {
            var navigation = scope.GetRequiredService<NavigationManager>();
            return new HttpClient { BaseAddress = new Uri(navigation.BaseUri) };
        }
    );

    builder.Services.AddControllers();

    builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

    var secret = builder.Configuration.GetValue<string>(Constants.JwtSecretName);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(
            opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "stuartmillman",
                    ValidAudience = "stuartmillman",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                };
            }
        );
    builder.Services.AddCascadingAuthenticationState();

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddScoped<ApiClient>();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    Log.Information("Building Application");
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.ConfigureLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapControllers();

    Log.Information("Running Application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}