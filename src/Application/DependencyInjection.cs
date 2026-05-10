using Application.Abstractions.Behaviors;
using Domain.Court.Service;
using Domain.Reservation.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
            
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        
        services.AddAutoMapper(cfg => {}, AppDomain.CurrentDomain.GetAssemblies());
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        return services;
    }
}

