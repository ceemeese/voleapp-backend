using Domain.Common.ValueObjects;
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
        var domainDayOfWeek = (Domain.Club.Enum.DayOfWeek)requestDateTime.DayOfWeek;
        var availableCourts = new List<Court>();
        
        foreach (var court in courts)
        {
            var club = clubs.FirstOrDefault(c => c.Id == court.ClubId);
            if (club == null) continue;

            //verificar Horario del Club
            var requestedTime = TimeOnly.FromDateTime(requestDateTime);
            var requestedEndTime = requestedTime.AddMinutes(durationMinutes);

            var isWithinSchedule = club.Schedules
                .Where(s => s.DayOfWeek == domainDayOfWeek)
                .Any(s => requestedTime >= s.OpeningTime && requestedEndTime <= s.ClosingTime);

            //si el club está cerrado para este horario, pasamos a la siguiente pista
            if (!isWithinSchedule) continue;

            //verificar disponibilidad física
            if (IsCourtAvailable(court, requestedRange, existingReservations))
            {
                availableCourts.Add(court);
            }
        }

        return Result.Success(availableCourts);
    }
    
    
    
    private bool IsCourtAvailable(Court court, DateTimeRange requestedRange, List<Reservation.Reservation> reservations)
    {
        //convertir el DateTime a DateOnly y TimeOnly para las reservas
        var requestedDate = DateOnly.FromDateTime(requestedRange.Start);
        var requestedStartTime = TimeOnly.FromDateTime(requestedRange.Start);
        var requestedEndTime = TimeOnly.FromDateTime(requestedRange.End);

        //hay conflicto con reservas existentes?
        bool hasReservationConflict = reservations
            .Where(r => r.CourtId == court.Id && r.Date == requestedDate)
            .Any(r => TimesOverlap(requestedStartTime, requestedEndTime, r.StartTime, r.EndTime));

        if (hasReservationConflict)
        {
            return false;
        }

        //hay conflicto con eventos?
        bool hasEventConflict = court.CourtEvents
            .Any(e => DateTimeOverlaps(requestedRange.Start, requestedRange.End, e.StartTime, e.EndTime));

        return !hasEventConflict;
    }

    private bool TimesOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && end1 > start2;
    }

    private bool DateTimeOverlaps(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && end1 > start2;
    }
}