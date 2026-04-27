using SharedKernel;

namespace Domain.Reservation.Services;

public sealed class ReservationService : IReservationService
{
    public Result<Reservation> BookCourt(Guid userId,
        Court.Court court,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes)
    {
        if (!court.IsActive)
        {
            return Result.Failure<Reservation>(ReservationErrors.CourtNotActive);
        }
        var duration = (end.ToTimeSpan() - start.ToTimeSpan()).TotalHours;
        var totalPrice = (decimal)duration * court.BasePrice;

        var reservationResult = Reservation.Create(userId, court.ClubId, court.Id, date, start, end, totalPrice, notes);
        if (reservationResult.IsFailure)
        {
            return Result.Failure<Reservation>(reservationResult.Error);
        }
        
        return Result.Success(reservationResult.Value);
    }
}