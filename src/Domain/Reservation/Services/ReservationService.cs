using Domain.Club;
using Domain.Club.Entities;
using Domain.Common.Services;
using Domain.Common.ValueObjects;
using Domain.Court;
using SharedKernel;

namespace Domain.Reservation.Services;

public sealed class ReservationService(IPricingService pricingService) : IReservationService
{
    public Result<Reservation> BookCourt(
        Guid userId,
        Court.Court court,
        Club.Club club,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes,
        WeatherData weather,
        PricingConfig pricingConfig)
    {
        if (!club.IsOpen(date, start, end))
        {
            return Result.Failure<Reservation>(ClubErrors.ClubClosed);
        }
        
        if (!court.IsActive)
        {
            return Result.Failure<Reservation>(CourtErrors.NotActive);
        }
        
        var eventCheck = court.CheckEventAvailability(date, start, end);
        if (eventCheck.IsFailure)
        {
            return Result.Failure<Reservation>(eventCheck.Error);
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