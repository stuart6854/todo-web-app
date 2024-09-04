using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(
            opts =>
            {
                // opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                opts.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            }
        );

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}