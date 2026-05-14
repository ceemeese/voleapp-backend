using Domain.Club.Entities;
using Domain.Common.Services;
using Domain.Common.ValueObjects;
using SharedKernel;

namespace Domain.Reservation.Services;

public interface IReservationService
{
    Result<Reservation> BookCourt(
        Guid userId,
        Court.Court court,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes,
        WeatherData weather,
        IPricingService pricingService,
        PricingConfig pricingConfig
    );
}
