using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application;

public static class Logging
{
    public static void InitializeLogging()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddSerilog();
        return services;
    }

    public static IApplicationBuilder ConfigureLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}