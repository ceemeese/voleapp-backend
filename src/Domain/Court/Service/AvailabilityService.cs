using Domain.Club;
using Domain.Common.ValueObjects;
using Domain.Common.Helpers;
using Domain.Reservation;
using SharedKernel;

namespace Domain.Court.Service;

public sealed class AvailabilityService : IAvailabilityService
{
    public Result<List<Court>> GetAvailableCourts(
        DateTime requestDateTime, 
        int durationMinutes, 
        string city, 
        List<Club.Club> clubs, 
        List<Court> courts, 
        List<Reservation.Reservation> existingReservations
        )
    {
        //validar fecha recibida
        var rangeResult = DateTimeRange.Create(requestDateTime, durationMinutes);
        if (rangeResult.IsFailure)
        {
            return Result.Failure<List<Court>>(rangeResult.Error);
        }
        
        var requestedRange = rangeResult.Value;
        var requestedDate = DateOnly.FromDateTime(requestedRange.Start);
        var requestedStartTime = TimeOnly.FromDateTime(requestedRange.Start);
        var requestedEndTime = TimeOnly.FromDateTime(requestedRange.End);
        
        var availableCourts = new List<Court>();
        
        foreach (var court in courts)
        {
            var club = clubs.FirstOrDefault(c => c.Id == court.ClubId);
            if (club is null) continue;

            var availabilityResult = CheckSlot(club, court, existingReservations, requestedDate, requestedStartTime, requestedEndTime);
            if (availabilityResult.IsFailure) continue;
            
            availableCourts.Add(court);
        }

        return Result.Success(availableCourts);
    }
    
    public Result CheckSlot(
        Club.Club club, 
        Court court, 
        List<Reservation.Reservation> reservations, 
        DateOnly date, 
        TimeOnly start, 
        TimeOnly end)
    {
        if (!club.IsOpen(date, start, end)) 
            return Result.Failure(ClubErrors.ClubClosed);

        if (!court.IsActive) 
            return Result.Failure(CourtErrors.NotActive);

        var eventCheck = court.CheckAvailability(date, start, end);
        if (eventCheck.IsFailure) return eventCheck;

        var hasConflict = reservations.Any(r => 
            r.CourtId == court.Id && 
            r.Date == date && 
            TimeOverlapHelper.TimesOverlap(start, end, r.StartTime, r.EndTime));

        return hasConflict 
            ? Result.Failure(ReservationErrors.TimeSlotOccupied) 
            : Result.Success();
    }
}