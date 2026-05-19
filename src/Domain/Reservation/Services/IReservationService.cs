using Domain.Club.Entities;
using Domain.Common.ValueObjects;
using SharedKernel;

namespace Domain.Reservation.Services;

public interface IReservationService
{
    Result<Reservation> BookCourt(
        Guid userId,
        Court.Court court,
        Club.Club club,
        List<Reservation> existingReservations,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes,
        WeatherData weather,
        PricingConfig pricingConfig
    );
}
