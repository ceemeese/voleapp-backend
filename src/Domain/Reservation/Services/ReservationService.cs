using Domain.Club.Entities;
using Domain.Common.Services;
using Domain.Common.ValueObjects;
using Domain.Court.Service;
using SharedKernel;

namespace Domain.Reservation.Services;

public sealed class ReservationService(IAvailabilityService availabilityService, IPricingService pricingService) : IReservationService
{
    public Result<Reservation> BookCourt(
        Guid userId,
        Court.Court court,
        Club.Club club,
        List<Reservation> existingReservations,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes,
        WeatherData weather,
        PricingConfig pricingConfig)
    {
        var availabilityResult = availabilityService.CheckSlot(club, court, existingReservations, date, start, end);
        if (availabilityResult.IsFailure)
        {
            return Result.Failure<Reservation>(availabilityResult.Error);
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