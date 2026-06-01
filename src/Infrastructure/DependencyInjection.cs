using System.Text;
using Application.Abstractions.Interfaces;
using Domain.User;
using Infrastructure.Authentication;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Persistence;
using Infrastructure.ServiceQueries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scrutor;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabase(configuration)
            .AddPersistenceInternal()
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddIdentityInternal()
            .AddWeatherApi()
            .AddEmailInternal()
            .AddQueries();

    
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<ApplicationDbContext>(options => options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

        services
            .AddDbContext<AuthDbContext>(options => options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

        return services;
    }

    private static IServiceCollection AddPersistenceInternal(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        services.Scan(scan => scan
            .FromAssemblies(typeof(ApplicationDbContext).Assembly)
            .AddClasses(
                filter => filter.Where(x => x.Name.EndsWith("Repository")),
                publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .AsMatchingInterface()
            .WithScopedLifetime());
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

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        var authError = UserErrors.NotAuthorized;

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var problem = new ProblemDetails
                        {
                            Title = authError.Code,
                            Detail = authError.Description,
                            Status = StatusCodes.Status401Unauthorized,
                            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                        };
                        await context.Response.WriteAsJsonAsync(problem);
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IClubContext, ClubContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        
        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();
        
        return services;
    }


    private static IServiceCollection AddIdentityInternal(this IServiceCollection services)
    {
            services.AddIdentityCore<AuthUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddWeatherApi(this IServiceCollection services)
    {
        services.AddHttpClient<IWeatherService, WeatherService.WeatherService>();
        return services;
    }

    private static IServiceCollection AddEmailInternal(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService.EmailService>();
        return services;
    }

    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IClubMemberQueries, ClubMemberQueries>();
        services.AddScoped<ICourtEventQueries, CourtEventQueries>();
        services.AddScoped<IReservationQueries, ReservationQueries>();
        services.AddScoped<IDashboardQueries, DashboardQueries>();
        services.AddScoped<IAnalyticsQueries, AnalyticsQueries>();
        services.AddScoped<IOccupancyQueries, OccupancyQueries>();

        return services;
    }
    
}