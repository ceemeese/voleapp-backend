using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabase(configuration)
            .AddPersistenceInternal()
            .AddHealthChecks(configuration);

    
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<ApplicationDbContext>(options => options
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        return services;
    }

    private static IServiceCollection AddPersistenceInternal(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration["ConnectionStrings__DefaultConnection"];
        }
        
        services
            .AddHealthChecks()
            .AddMySql(connectionString!);
        return services;
    }
    
    public static void ApplyMigrations(this IHost app)
    {
        var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory?.CreateScope())
        {
            var dbContext = scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext?.Database.Migrate();
        }
    }
        
}