using Domain.Club.Entities;
using Domain.Common.ValueObjects;
using Domain.Court.Entities;
using SharedKernel;

namespace Domain.Court.Service;

public interface IAvailabilityService
{
    Result<List<Court>> GetAvailableCourts(DateTime requestDateTime, int durationMinutes, string city, List<Club.Club> club, List<Court> court, List<Reservation.Reservation> existingReservations);
}