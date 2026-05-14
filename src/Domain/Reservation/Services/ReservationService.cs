using Domain.Club.Entities;
using Domain.Common.Services;
using Domain.Common.ValueObjects;
using SharedKernel;

namespace Domain.Reservation.Services;

public sealed class ReservationService : IReservationService
{
    public Result<Reservation> BookCourt(
        Guid userId,
        Court.Court court,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes,
        WeatherData weather,
        IPricingService pricingService,
        PricingConfig pricingConfig)
    {
        if (!court.IsActive)
        {
            return Result.Failure<Reservation>(ReservationErrors.CourtNotActive);
        }
        
        var totalPrice = pricingService.CalculateTransactionPrice(
            court.BasePrice, 
            pricingConfig, 
            weather, 
            start, 
            end);
        
        var reservationResult = Reservation.Create(userId, court.ClubId, court.Id, date, start, end, totalPrice, notes);
        if (reservationResult.IsFailure)
        {
            return Result.Failure<Reservation>(reservationResult.Error);
        }
        
        return Result.Success(reservationResult.Value);
    }
}